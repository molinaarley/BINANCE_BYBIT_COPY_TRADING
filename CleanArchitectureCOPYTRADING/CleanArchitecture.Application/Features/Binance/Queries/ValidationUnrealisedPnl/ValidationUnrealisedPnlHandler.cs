using System.Globalization;
using System.Reflection;
using AutoMapper;
using CleanArchitecture.Application.Contracts.Infrastructure;
using CleanArchitecture.Application.Contracts.Persistence;
using CleanArchitecture.Application.Converters;
using CleanArchitecture.Application.Models;
using CleanArchitecture.Domain.Binance;
using CleanArchitecture.Domain.Enum;
using MediatR;
using Newtonsoft.Json;

namespace CleanArchitecture.Application.Features.Binance.Queries.LoadPosition
{
    public class ValidationUnrealisedPnlHandler : IRequestHandler<ValidationUnrealisedPnlQuery, int>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly ITelegrameBotService _telegrameBotService;
        private string _pathDataJson = @"C:\worck\BINANCE_DATA\";
        private string _pathDataJsonZip = @"C:\worck\BINANCE_DATA_ZIP\";
        private readonly IDonetZipService _donetZipService;
        private readonly IFileService _fileService;
        private readonly IByBitApiService _byBitApiService;
        private readonly IBinanceTraderService _binanceTraderService;
        private readonly IBinanceOrderService _binanceOrderService;
        private readonly IBinanceByBitUsersService _binanceByBitUsersService;
        private readonly IBinanceByBitOrderService _binanceByBitOrderService;
        private readonly IBinanceMonitoringProcessService _binanceMonitoringProcessService;








        public ValidationUnrealisedPnlHandler(IUnitOfWork unitOfWork,  ITelegrameBotService telegrameBotService,
            IDonetZipService donetZipService, IFileService fileService, IByBitApiService byBitService,
            IBinanceTraderService binanceTraderService, IBinanceOrderService binanceOrderService,
            IBinanceByBitUsersService binanceByBitUsersService,
            IBinanceByBitOrderService binanceByBitOrderService,
            IBinanceMonitoringProcessService binanceMonitoringProcessService)
        {
            //_videoRepository = videoRepository;
            _unitOfWork = unitOfWork;
            _telegrameBotService = telegrameBotService ?? throw new ArgumentException(nameof(telegrameBotService));
            _donetZipService = donetZipService ?? throw new ArgumentException(nameof(donetZipService));
            _fileService = fileService ?? throw new ArgumentException(nameof(fileService));
            _byBitApiService = byBitService ?? throw new ArgumentException(nameof(byBitService));
            _binanceTraderService = binanceTraderService ?? throw new ArgumentException(nameof(binanceTraderService));
            _binanceOrderService = binanceOrderService ?? throw new ArgumentException(nameof(binanceOrderService));
            _binanceByBitUsersService = binanceByBitUsersService ?? throw new ArgumentException(nameof(binanceByBitUsersService));
            _binanceByBitOrderService = binanceByBitOrderService ?? throw new ArgumentException(nameof(binanceByBitOrderService));
            _binanceMonitoringProcessService = binanceMonitoringProcessService ?? throw new ArgumentException(nameof(binanceMonitoringProcessService));
        }

        public async Task<int> Handle(ValidationUnrealisedPnlQuery request, CancellationToken cancellationToken)
        {
            var allBinanceOrder = await _binanceOrderService.GetAll();
            //OBTENIR DES UTILISATEURS QUI ONT PAYÉ LA REDEVANCE D'ABONNEMENT
            var allUserActive =await _binanceByBitUsersService.GetAllIsactive();

            foreach (var itemUser in allUserActive)
            {

                //NOUS OBTENONS LE MONTANT D'ARGENT QUE VOUS AVEZ SUR VOTRE COMPTE BYBIT
                var walletBalance = await _byBitApiService.WalletBalance(itemUser.ApiKey, itemUser.SecretKey);
                var coinWalletBalance = await _byBitApiService.GetCoinFromWalletBalance(walletBalance);
                if (coinWalletBalance.unrealisedPnl.Value > 3000)
                {
                    /* var getFeeRateInfo = await _byBitApiService.GetFeeRate(new GetFeeRateQuery()
                     {
                         category = "linear",
                         apiKey = itemUser.ApiKey,
                         secretKey = itemUser.SecretKey
                     });*/

                    var getClosedPnLList = await _byBitApiService.GetClosedPnL(new GetClosedPnL()
                    {
                        category = "linear",
                        apiKey = itemUser.ApiKey,
                        secretKey = itemUser.SecretKey
                    });


                    //delete all position
                    foreach (var itemBinanceOrderBD in allBinanceOrder)
                    {



                        foreach (var itemOritemBinanceByBitOrders in itemBinanceOrderBD.BinanceByBitOrders)
                        {
                            //if (itemOritemBinanceByBitOrders.IdTelegrame == itemUser.IdTelegrame)
                            {

                                var positionBybitInfo = await _byBitApiService.GetPositionInfo(new GetPositionInfo()
                                {
                                    category = "linear",
                                    symbol = itemBinanceOrderBD.Symbol,
                                    apiKey = itemUser.ApiKey,
                                    secretKey = itemUser.SecretKey
                                });

                                //nous créons la position broker  bybit
                                if (positionBybitInfo != null && positionBybitInfo.list != null && positionBybitInfo.list.Any())
                                {
                                    PlaceOrder placeOrder = new PlaceOrder();
                                    placeOrder.symbol = positionBybitInfo.list.FirstOrDefault().symbol;//itemPosition.symbol;
                                    placeOrder.orderType = OrderType.Market.ToString();


                                    if (positionBybitInfo.list.FirstOrDefault().side.Equals(EnumConverter.GetString(OrderSide.Buy)))
                                    {
                                        placeOrder.side = EnumConverter.GetString(OrderSide.Sell);
                                    }
                                    if (positionBybitInfo.list.FirstOrDefault().side.Equals(EnumConverter.GetString(OrderSide.Sell)))
                                    {
                                        placeOrder.side = EnumConverter.GetString(OrderSide.Buy);
                                    }

                                    if (positionBybitInfo.list.FirstOrDefault().side.Equals(EnumConverter.GetString(OrderSide.None)))
                                    {
                                        placeOrder.side = EnumConverter.GetString(OrderSide.None);
                                    }
                                    placeOrder.category = EnumConverter.GetString(Category.Linear);
                                    //"linear";
                                    placeOrder.apiKey = itemUser.ApiKey;
                                    placeOrder.secretKey = itemUser.SecretKey;
                                    placeOrder.timeInForce = EnumConverter.GetString(TimeInForce.GoodTillCanceled);
                                    placeOrder.reduceOnly = true;
                                    placeOrder.closeOnTrigger = true;
                                    placeOrder.qty = itemBinanceOrderBD.Amount < 0 ? itemBinanceOrderBD.Amount * -1 : itemBinanceOrderBD.Amount;
                                    var placeOrderByBit = await _byBitApiService.PlaceOrder(placeOrder);
                                    await _binanceByBitOrderService.Deleted(itemOritemBinanceByBitOrders);
                                    File.AppendAllLines("AllSupresionValidationUnrealisedPnlQuery.txt", new[] { string.Concat(" deleted  symbol/guid : ", placeOrder.symbol, "#", itemBinanceOrderBD.EncryptedUid, " DATE :", DateTime.Now.ToString()) });
                                }

                            }
                        }
                    }
                    await _byBitApiService.CreateInternalTransfer(new CreateInternalTransferQuery()
                    {
                        ApiKey = itemUser.ApiKey,
                        SecretKey = itemUser.SecretKey,
                        Amount = coinWalletBalance.unrealisedPnl.Value.ToString(CultureInfo.InvariantCulture),
                        Coin = "USDT",
                        FromAccountType = EnumConverter.GetString(AccountType.Contract),
                        ToAccountType = EnumConverter.GetString(AccountType.Spot),
                    });
                }
            }
           
            return 1;
        }
    }
}
