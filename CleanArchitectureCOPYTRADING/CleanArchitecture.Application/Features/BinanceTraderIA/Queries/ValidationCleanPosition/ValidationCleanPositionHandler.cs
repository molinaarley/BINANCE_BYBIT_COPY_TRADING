using System.Text;
using System;
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
using Microsoft.EntityFrameworkCore.Metadata;

namespace CleanArchitecture.Application.Features.Binance.Queries.LoadPosition
{
    public class ValidationCleanPositionHandler : IRequestHandler<ValidationCleanPositionQuery, int>
    {
        private readonly ITelegrameBotService _telegrameBotService;
        private string _pathDataJson = @"C:\worck\BINANCE_DATA\";
        private readonly IByBitApiService _byBitApiService;
        private readonly IBinanceOrderService _binanceOrderService;
        private readonly IBinanceByBitUsersService _binanceByBitUsersService;
        private readonly IBinanceByBitOrderService _binanceByBitOrderService;



        public ValidationCleanPositionHandler( ITelegrameBotService telegrameBotService,
             IByBitApiService byBitService,
            IBinanceOrderService binanceOrderService,
            IBinanceByBitUsersService binanceByBitUsersService,
            IBinanceByBitOrderService binanceByBitOrderService)
        {
            //_videoRepository = videoRepository;
            _telegrameBotService = telegrameBotService ?? throw new ArgumentException(nameof(telegrameBotService));
            _byBitApiService = byBitService ?? throw new ArgumentException(nameof(byBitService));
            _binanceOrderService = binanceOrderService ?? throw new ArgumentException(nameof(binanceOrderService));
            _binanceByBitUsersService = binanceByBitUsersService ?? throw new ArgumentException(nameof(binanceByBitUsersService));
            _binanceByBitOrderService = binanceByBitOrderService ?? throw new ArgumentException(nameof(binanceByBitOrderService));

        }

        public async Task<int> Handle(ValidationCleanPositionQuery request, CancellationToken cancellationToken)
        {
            

            

            int totalDeleted = 0;

            List<PositionData> positionsData = await _telegrameBotService.LoadPosition(_pathDataJson);
            if (positionsData==null)
            {
                
               // return 0;
            }
            //ByBitApiV3Service
            List<OtherPositionRetList> otherPositionRetList = await _binanceOrderService.GetOtherPositionRetList(positionsData);

            //OBTENIR DES UTILISATEURS QUI ONT PAYÉ LA REDEVANCE D'ABONNEMENT
            var allUserActive = await _binanceByBitUsersService.GetAllIsactive();

            foreach (var itemUser in allUserActive)
            {
                //NOUS OBTENONS LE MONTANT D'ARGENT QUE VOUS AVEZ SUR VOTRE COMPTE BYBIT
                var walletBalance = await _byBitApiService.WalletBalance(itemUser.ApiKey, itemUser.SecretKey);
                var coinWalletBalance = await _byBitApiService.GetCoinFromWalletBalance(walletBalance);
                /*  var getClosedPnLList = await _byBitApiService.GetClosedPnL(new GetClosedPnL()
                  {
                      category = "linear",
                      apiKey = itemUser.ApiKey,
                      secretKey = itemUser.SecretKey
                  });*/

                var getFeeRateInfo = await _byBitApiService.GetFeeRate(new GetFeeRateQuery()
                {
                    category = "linear",
                    apiKey = itemUser.ApiKey,
                    secretKey = itemUser.SecretKey
                });

                var result = otherPositionRetList.ToDictionary(p => p.symbol, p => p.symbol);

                List<BinanceOrder> binanceOrderBD = await _binanceOrderService.GetAll();

                foreach (var itembinanceOrder in binanceOrderBD)
                {

                    if (!result.ContainsKey(itembinanceOrder.Symbol))
                    {
                        var positionBybitInfo = await _byBitApiService.GetPositionInfo(new GetPositionInfo()
                        {
                            category = EnumConverter.GetString(Category.Linear),//"linear",
                            symbol = itembinanceOrder.Symbol,
                            apiKey = itemUser.ApiKey,
                            secretKey = itemUser.SecretKey
                        });
                        if (positionBybitInfo != null && positionBybitInfo.list != null && positionBybitInfo.list.Any() &&
                                !positionBybitInfo.list.FirstOrDefault().side.Equals(EnumConverter.GetString(OrderSide.None)))
                        {
                            if (positionBybitInfo != null && positionBybitInfo.list != null && positionBybitInfo.list.Any())
                            {
                                PlaceOrder placeOrder = new PlaceOrder();
                                placeOrder = await _binanceOrderService.GetPlaceOrderForDeleted(positionBybitInfo, itemUser.ApiKey, itemUser.SecretKey, 0);

                                var placeOrderByBit = await _byBitApiService.PlaceOrder(placeOrder);
                                totalDeleted++;

                                //itembinanceOrder.BinanceByBitOrders
                                await _binanceByBitOrderService.DeletedRange(itembinanceOrder.BinanceByBitOrders.ToList());
                                await _binanceOrderService.DeletedOrder(itembinanceOrder);

                                File.AppendAllLines("ValidationCleanPositionHandlerlog.txt", new[] { string.Concat("deted from bybit symbol :", placeOrder.symbol, "*******position deleted*********", DateTime.Now.ToString("yyyy/MM/dd HH:mm:ss:fff")) });
                                File.AppendAllLines("ValidationCleanPositionHandlerlog.txt", new[] { string.Concat(" placeOrder.side  => ", placeOrder.side) });
                            }
                        }
                    }


                }
            }
            File.AppendAllLines("WriteLinesLog.txt", new[] { "*******total Deleted position un plus************" });
            File.AppendAllLines("WriteLinesLog.txt", new[] { string.Concat("total => ", totalDeleted.ToString()) });
            return totalDeleted;
        }
    }
}
