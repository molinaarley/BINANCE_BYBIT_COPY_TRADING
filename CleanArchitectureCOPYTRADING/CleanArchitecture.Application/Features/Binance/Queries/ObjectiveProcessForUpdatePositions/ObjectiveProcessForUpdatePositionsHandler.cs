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
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Caching.Memory;

namespace CleanArchitecture.Application.Features.Binance.Queries.ObjectiveProcessForUpdatePositions
{
    public class ValidationCleanPositionHandler : IRequestHandler<ObjectiveProcessForUpdatePositionsQuery, int>
    {
        private readonly ITelegrameBotService _telegrameBotService;
        private string _pathDataJson = @"C:\worck\BINANCE_DATA\";
        private readonly IByBitApiService _byBitApiService;
        private readonly IBinanceOrderService _binanceOrderService;
        private readonly IBinanceByBitUsersService _binanceByBitUsersService;
        private readonly IBinanceByBitOrderService _binanceByBitOrderService;
        private readonly IBinanceMonitoringCoinWalletBalanceObjectiveProcessService _monitoringCoinWalletBalanceObjectiveProcessService;
 
        private readonly IBinanceCacheByBitSymbolService _binanceCacheByBitSymbolService;
        public IConfiguration _configuration { get; }



        public ValidationCleanPositionHandler(ITelegrameBotService telegrameBotService,
             IByBitApiService byBitService,
            IBinanceOrderService binanceOrderService,
            IBinanceByBitUsersService binanceByBitUsersService,
            IBinanceByBitOrderService binanceByBitOrderService, IBinanceMonitoringCoinWalletBalanceObjectiveProcessService monitoringCoinWalletBalanceObjectiveProcessService, IConfiguration configuration,
        IBinanceCacheByBitSymbolService binanceCacheByBitSymbolService)
        {
            //_videoRepository = videoRepository;
            _telegrameBotService = telegrameBotService ?? throw new ArgumentException(nameof(telegrameBotService));
            _byBitApiService = byBitService ?? throw new ArgumentException(nameof(byBitService));
            _binanceOrderService = binanceOrderService ?? throw new ArgumentException(nameof(binanceOrderService));
            _binanceByBitUsersService = binanceByBitUsersService ?? throw new ArgumentException(nameof(binanceByBitUsersService));
            _binanceByBitOrderService = binanceByBitOrderService ?? throw new ArgumentException(nameof(binanceByBitOrderService));
            _monitoringCoinWalletBalanceObjectiveProcessService = monitoringCoinWalletBalanceObjectiveProcessService ?? throw new ArgumentException(nameof(monitoringCoinWalletBalanceObjectiveProcessService));
            _configuration = configuration ?? throw new ArgumentException(nameof(configuration));
            _binanceCacheByBitSymbolService = binanceCacheByBitSymbolService ?? throw new ArgumentException(nameof(binanceCacheByBitSymbolService));
        }

        public async Task<int> Handle(ObjectiveProcessForUpdatePositionsQuery request, CancellationToken cancellationToken)
        {
            int totalDeleted = 0;
         
            //OBTENIR DES UTILISATEURS QUI ONT PAYÉ LA REDEVANCE D'ABONNEMENT
            var allUserActive = await _binanceByBitUsersService.GetAllIsactive();
            List<BinanceOrder> binanceOrderBD = await _binanceOrderService.GetAll();

            foreach (var itemUser in allUserActive)
            {
                //NOUS OBTENONS LE MONTANT D'ARGENT QUE VOUS AVEZ SUR VOTRE COMPTE BYBIT
                var walletBalance = await _byBitApiService.WalletBalance(itemUser.ApiKey, itemUser.SecretKey);
                var coinWalletBalance = await _byBitApiService.GetCoinFromWalletBalance(walletBalance);
           
                var getFeeRateInfo = await _byBitApiService.GetFeeRate(new GetFeeRateQuery()
                {
                    category = "linear",
                    apiKey = itemUser.ApiKey,
                    secretKey = itemUser.SecretKey
                });

              
                var getInstrumentsInfo = (await _byBitApiService.GetInstrumentsInfo(new GetInstrumentsInfoQuery()
                {
                    apiKey = itemUser.ApiKey,
                    secretKey = itemUser.SecretKey,
                    category = EnumConverter.GetString(Category.Linear)
                })).result.list.GroupBy(x => x.symbol).ToDictionary(p => p.Key, p => p.FirstOrDefault());


             
                //verificamos si estamos buscando el objetico
                bool IsIngProcessForObjective = (await _monitoringCoinWalletBalanceObjectiveProcessService.IsIngProcessForObjective(long.Parse(_configuration.GetSection("BinanceBybitSettings:IdTelegrameMain").Value), coinWalletBalance));
                if (!IsIngProcessForObjective)
                {
                    BinanceMonitoringCoinWalletBalanceObjectiveProcess LastIsIngProcessForObjective = await _monitoringCoinWalletBalanceObjectiveProcessService.GetLastIsIngProcessForObjective(long.Parse(_configuration.GetSection("BinanceBybitSettings:IdTelegrameMain").Value));
                    if (LastIsIngProcessForObjective!=null && !LastIsIngProcessForObjective.EndDate.HasValue)
                    {
                        LastIsIngProcessForObjective.EndDate = DateTime.Now;
                        await _monitoringCoinWalletBalanceObjectiveProcessService.Update(LastIsIngProcessForObjective);
                    }
                }
                

                foreach (var itembinanceOrder in binanceOrderBD)
                {
                    if ( !(await _monitoringCoinWalletBalanceObjectiveProcessService.IsIngProcessForObjective(long.Parse(_configuration.GetSection("BinanceBybitSettings:IdTelegrameMain").Value), coinWalletBalance) ) )
                    {

                        var tickersCategorySymbol = await _byBitApiService.GetTickersCategorySymbol(new GetTickers() { category = "inverse", symbol = itembinanceOrder.Symbol });

                        var positionBybitInfo = await _byBitApiService.GetPositionInfo(new GetPositionInfo()
                        {
                            category = EnumConverter.GetString(Category.Linear),//"linear",
                            symbol = itembinanceOrder.Symbol,
                            apiKey = itemUser.ApiKey,
                            secretKey = itemUser.SecretKey
                        });
                        if (positionBybitInfo != null && positionBybitInfo.list != null && positionBybitInfo.list.Any() &&
                                !positionBybitInfo.list.FirstOrDefault().side.Equals(EnumConverter.GetString(OrderSide.None)) && !string.IsNullOrEmpty( positionBybitInfo.list.FirstOrDefault().side) )
                        {
                            if (positionBybitInfo != null && positionBybitInfo.list != null && positionBybitInfo.list.Any()  && !(await _binanceCacheByBitSymbolService.HasSymbolInCache(itembinanceOrder.Symbol)))
                            {
                                PlaceOrder placeOrder = new PlaceOrder();
                                placeOrder = await _binanceOrderService.GetPlaceOrderForDeleted(positionBybitInfo, itemUser.ApiKey, itemUser.SecretKey, 0);
                                placeOrder.orderType = OrderType.Limit.ToString();

                                if (positionBybitInfo.list.FirstOrDefault().side.Equals(EnumConverter.GetString(OrderSide.Buy)))
                                {
                                    placeOrder.price = positionBybitInfo.list.FirstOrDefault().markPrice - (positionBybitInfo.list.FirstOrDefault().markPrice * getInstrumentsInfo[placeOrder.symbol].priceFilter.tickSize);

                                }

                                if (positionBybitInfo.list.FirstOrDefault().side.Equals(EnumConverter.GetString(OrderSide.Sell)))
                                {
                                    placeOrder.price = positionBybitInfo.list.FirstOrDefault().markPrice + (positionBybitInfo.list.FirstOrDefault().markPrice * getInstrumentsInfo[placeOrder.symbol].priceFilter.tickSize);
                                }

                                if (positionBybitInfo.list.FirstOrDefault().side.Equals(EnumConverter.GetString(OrderSide.None)))
                                {
                                    placeOrder.price = positionBybitInfo.list.FirstOrDefault().markPrice;
                                }

                                if (string.IsNullOrEmpty(positionBybitInfo.list.FirstOrDefault().side))
                                {
                                    placeOrder.price = positionBybitInfo.list.FirstOrDefault().markPrice;
                                    placeOrder.side = EnumConverter.GetString(OrderSide.None);
                                }


                                if (tickersCategorySymbol.result.list.FirstOrDefault().lastPrice.Value.ToString(CultureInfo.InvariantCulture).Contains("."))
                                {
                                    placeOrder.price = Math.Round(placeOrder.price.Value, tickersCategorySymbol.result.list.FirstOrDefault().lastPrice.Value.ToString(CultureInfo.InvariantCulture).Split('.')[1].Length, MidpointRounding.ToEven);
                             
                                }
                                else
                                {
                                    placeOrder.price = Math.Round(placeOrder.price.Value, 0, MidpointRounding.ToEven);
                                }



                                var placeOrderByBit = await _byBitApiService.PlaceOrder(placeOrder);
                                totalDeleted++;

                                //itembinanceOrder.BinanceByBitOrders
                                await _binanceByBitOrderService.DeletedRange(itembinanceOrder.BinanceByBitOrders.ToList());
                                await _binanceOrderService.DeletedOrder(itembinanceOrder);

                                File.AppendAllLines("ValidationCleanPositionHandlerlog.txt", new[] { string.Concat("deted from bybit symbol (Handle MONITORING) :", placeOrder.symbol, "*******position deleted*********", DateTime.Now.ToString("yyyy/MM/dd HH:mm:ss:fff")) });
                                File.AppendAllLines("ValidationCleanPositionHandlerlog.txt", new[] { string.Concat(" placeOrder.side (Handle MONITORING) => ", placeOrder.side) });
                            }
                        }
                    }


                }
            }
            
            return totalDeleted;
        }
        
    }
}
