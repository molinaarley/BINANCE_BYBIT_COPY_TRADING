
using System.Collections.Concurrent;
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

namespace CleanArchitecture.Application.Features.Binance.Queries.CleanPosition
{
    public class RegulationPositionHandler : IRequestHandler<RegulationPositionQuery, int>
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

        
        public RegulationPositionHandler(IUnitOfWork unitOfWork, ITelegrameBotService telegrameBotService,
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

        public async Task<int> Handle(RegulationPositionQuery request, CancellationToken cancellationToken)
        {
            //OBTENIR DES UTILISATEURS QUI ONT PAYÉ LA REDEVANCE D'ABONNEMENT
            var allUserActive = await _binanceByBitUsersService.GetAllIsactive();
            var allBinanceOrder = (await _binanceOrderService.GetAll());
            double totalLosses = 0;
            foreach (var itemBinanceOrderBD in allBinanceOrder)
            {
                foreach (var itemUser in allUserActive)
                {
                   
                    foreach (var itemOritemBinanceByBitOrders in itemBinanceOrderBD.BinanceByBitOrders)
                    {
                        var positionBybitInfo = await _byBitApiService.GetPositionInfo(new GetPositionInfo()
                        {
                            category = "linear",
                            symbol = itemBinanceOrderBD.Symbol, 
                            apiKey = itemUser.ApiKey,
                            secretKey = itemUser.SecretKey
                        });

                        //n est pas en BD  don il faut eliminer la position
                        if (positionBybitInfo != null && positionBybitInfo.list != null && positionBybitInfo.list.Any() &&
                            !positionBybitInfo.list.FirstOrDefault().side.Equals(EnumConverter.GetString(OrderSide.None)))
                        {
                           var firstPositionBybitInfo = positionBybitInfo.list.FirstOrDefault();
                            double? realizedLosses = 0;


                            if (firstPositionBybitInfo.unrealisedPnl < 0)
                            {
                                totalLosses = totalLosses + (firstPositionBybitInfo.unrealisedPnl.Value );


                            }
                            else
                            {
                                File.AppendAllLines("GananciasParPosition.txt", new[] { string.Concat(firstPositionBybitInfo.unrealisedPnl) });

                            }
                            if (firstPositionBybitInfo.unrealisedPnl>5000)///-50
                            {
                               
                                realizedLosses = (  (firstPositionBybitInfo.unrealisedPnl* -1) * 100) / firstPositionBybitInfo.positionValue;

                                if (true)//realizedLosses>5
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
                                    placeOrder.qty = 0;//itemBinanceOrderBD.Amount;
                                    var placeOrderByBit = await _byBitApiService.PlaceOrder(placeOrder);
                                    File.AppendAllLines("SYMBOLOAS_A_NO_IMPORTAR.txt", new[] { string.Concat(placeOrder.symbol) });


                                    File.AppendAllLines("WriteLinesLogPositionEnGananciasNoImportar.txt", new[] {  "symbole :", placeOrder.symbol," ", string.Concat( ( firstPositionBybitInfo.unrealisedPnl * -1), DateTime.Now.ToString("yyyy/MM/dd HH:mm:ss:fff")) });

                                }
                            }
                        }
                       // await _binanceByBitOrderService.Deleted(itemOritemBinanceByBitOrders);
                    }


                    File.AppendAllLines("TotalPerdidas.txt", new[] { string.Concat( totalLosses) });

                }
                //await _binanceOrderService.DeletedOrder(itemBinanceOrderBD);
            }
            return 0;
        }
    }
}
