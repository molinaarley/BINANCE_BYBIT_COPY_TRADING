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
using System.Linq;

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
        private readonly IBinanceMonitoringCoinWalletBalanceObjectiveProcessService _monitoringCoinWalletBalanceObjectiveProcessService;
        private readonly IBinanceTraderUrlForUpdatePositionBinanceQueryService _binanceTraderUrlForUpdatePositionBinanceQueryService;
        public IConfiguration _configuration { get; }



        public ValidationCleanPositionHandler(ITelegrameBotService telegrameBotService,
             IByBitApiService byBitService,
            IBinanceOrderService binanceOrderService,
            IBinanceByBitUsersService binanceByBitUsersService,
            IBinanceByBitOrderService binanceByBitOrderService, IBinanceMonitoringCoinWalletBalanceObjectiveProcessService monitoringCoinWalletBalanceObjectiveProcessService,
             IBinanceTraderUrlForUpdatePositionBinanceQueryService binanceTraderUrlForUpdatePositionBinanceQueryService,
            IConfiguration configuration)
        {
            //_videoRepository = videoRepository;
            _telegrameBotService = telegrameBotService ?? throw new ArgumentException(nameof(telegrameBotService));
            _byBitApiService = byBitService ?? throw new ArgumentException(nameof(byBitService));
            _binanceOrderService = binanceOrderService ?? throw new ArgumentException(nameof(binanceOrderService));
            _binanceByBitUsersService = binanceByBitUsersService ?? throw new ArgumentException(nameof(binanceByBitUsersService));
            _binanceByBitOrderService = binanceByBitOrderService ?? throw new ArgumentException(nameof(binanceByBitOrderService));
            _monitoringCoinWalletBalanceObjectiveProcessService = monitoringCoinWalletBalanceObjectiveProcessService ?? throw new ArgumentException(nameof(monitoringCoinWalletBalanceObjectiveProcessService));
            _binanceTraderUrlForUpdatePositionBinanceQueryService = binanceTraderUrlForUpdatePositionBinanceQueryService ?? throw new ArgumentException(nameof(binanceTraderUrlForUpdatePositionBinanceQueryService));
            _configuration = configuration ?? throw new ArgumentException(nameof(configuration));
        }

        public async Task<int> Handle(ValidationCleanPositionQuery request, CancellationToken cancellationToken)
        {




            int totalDeleted = 0;

            List<PositionData> positionsData = await _telegrameBotService.LoadPosition(_pathDataJson);
            Dictionary<string, string> keyFromFile  = positionsData.GroupBy(p => p.code).ToDictionary(p => p.Key, p => p.FirstOrDefault().code);
        
            if (positionsData == null)
            {

                // return 0;
            }
            //ByBitApiV3Service
            List<OtherPositionRetList> otherPositionRetList = await _binanceOrderService.GetOtherPositionRetList(positionsData);

            //OBTENIR DES UTILISATEURS QUI ONT PAYÉ LA REDEVANCE D'ABONNEMENT
            var allUserActive = await _binanceByBitUsersService.GetAllIsactive();
            List<BinanceOrder> binanceOrderBD = await _binanceOrderService.GetAll();

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

                Dictionary<string, string> result = new Dictionary<string, string>();
                result = otherPositionRetList.GroupBy(p => p.symbol).ToDictionary(p => p.Key, p => p.FirstOrDefault().symbol);

                var getInstrumentsInfo = (await _byBitApiService.GetInstrumentsInfo(new GetInstrumentsInfoQuery()
                {
                    apiKey = itemUser.ApiKey,
                    secretKey = itemUser.SecretKey,
                    category = EnumConverter.GetString(Category.Linear)
                })).result.list.GroupBy(x => x.symbol).ToDictionary(p => p.Key, p => p.FirstOrDefault());


                //update positiones Satellite GetInstrumentsInfoRoot
                await UpdatePositionSatellite(getInstrumentsInfo, itemUser, binanceOrderBD.GroupBy(x =>  x.Symbol).ToDictionary(p => p.Key, p => p.FirstOrDefault()) , coinWalletBalance );


                var resultTraderUrlForUpdatePositionBinance = await _binanceTraderUrlForUpdatePositionBinanceQueryService.GetTraderUrlForUpdatePositionBinance();
                var totalTraderUrlForUpdatePositionBinanceQuery = resultTraderUrlForUpdatePositionBinance.Select(p => p.EncryptedUid).Take(int.Parse(_configuration.GetSection("BinanceBybitSettings:TotalTraderUrlForUpdatePositionBinanceQuery").Value)).ToList();


                

                foreach (var itembinanceOrder in binanceOrderBD)
                {

                 


                    //aki falta recuprar la order si la tiene puesta
                    //que cierra la posicion si no la tiene puesta en ese caso se pondria
                    //es utilizando esta funcion  var hasOrderOpenForThisSymbol = await _byBitApiService.GetOpenOrders.......
                    //tratar de utilizar la opcion recuce only
                    if (keyFromFile.ContainsKey(itembinanceOrder.EncryptedUid)  && (!result.ContainsKey(itembinanceOrder.Symbol) ||
                        !(await _monitoringCoinWalletBalanceObjectiveProcessService.IsIngProcessForObjective(itemUser.IdTelegrame, coinWalletBalance) ))
                        )
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
                                !positionBybitInfo.list.FirstOrDefault().side.Equals(EnumConverter.GetString(OrderSide.None)))
                        {
                          

                            
                                if (positionBybitInfo != null && positionBybitInfo.list != null && positionBybitInfo.list.Any())
                                {
                                    PlaceOrder placeOrder = new PlaceOrder();
                                    placeOrder = await _binanceOrderService.GetPlaceOrderForDeleted(positionBybitInfo, itemUser.ApiKey, itemUser.SecretKey, 0);
                                    placeOrder.orderType = OrderType.Limit.ToString();

                                    if (positionBybitInfo.list.FirstOrDefault().side.Equals(EnumConverter.GetString(OrderSide.Buy)) && getInstrumentsInfo.ContainsKey(placeOrder.symbol))
                                    {
                                        placeOrder.price = positionBybitInfo.list.FirstOrDefault().markPrice - (positionBybitInfo.list.FirstOrDefault().markPrice * getInstrumentsInfo[placeOrder.symbol].priceFilter.tickSize);

                                    }

                                    if (positionBybitInfo.list.FirstOrDefault().side.Equals(EnumConverter.GetString(OrderSide.Sell)) && getInstrumentsInfo.ContainsKey(placeOrder.symbol))
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

                                    if (placeOrder.price.HasValue)
                                    {
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

                                        File.AppendAllLines("ValidationCleanPositionHandlerlog.txt", new[] { string.Concat("deted from bybit symbol (Handle) :", placeOrder.symbol, "*******position deleted*********", DateTime.Now.ToString("yyyy/MM/dd HH:mm:ss:fff")) });
                                        File.AppendAllLines("ValidationCleanPositionHandlerlog.txt", new[] { string.Concat(" placeOrder.side (Handle) => ", placeOrder.side) });
                                    }
                                }
           
                        }
                    }


                }
            }
            File.AppendAllLines("WriteLinesLog.txt", new[] { "*******total Deleted position un plus************" });
            File.AppendAllLines("WriteLinesLog.txt", new[] { string.Concat("total => ", totalDeleted.ToString()) });
            return totalDeleted;
        }


        /// <summary>
        /// update position satellite(It is a position that is found on binance but it is not in BD)
        /// </summary>
        /// <param name="byBitUser"></param>
        /// <param name="binanceOrderBD"></param>
        /// <returns></returns>
        private async Task<bool> UpdatePositionSatellite(Dictionary<string, GetInstrumentsInfoList> getInstrumentsInfo, BinanceByBitUser byBitUser, Dictionary<string,BinanceOrder> binanceOrderBD, Coin coinWalletBalance)
        {
           // _binanceOrderService.GetAll();

            List<BinanceOrderAudit> binanceOrderAudit = await _binanceOrderService.GetByDateMax(DateTime.Now.AddDays(-5));

            List<string> allsymbol = new List<string>();
            allsymbol = binanceOrderAudit.Select(p=>p.Symbol).ToList();

            allsymbol.AddRange((await _binanceOrderService.GetAll()).Select(p => p.Symbol).ToList());
            allsymbol = allsymbol.Distinct().ToList();

            //verificamos si estamos buscando el objetico
            bool IsIngProcessForObjective = (await _monitoringCoinWalletBalanceObjectiveProcessService.IsIngProcessForObjective(byBitUser.IdTelegrame, coinWalletBalance));
            if (!IsIngProcessForObjective  )
            {
                BinanceMonitoringCoinWalletBalanceObjectiveProcess LastIsIngProcessForObjective = await _monitoringCoinWalletBalanceObjectiveProcessService.GetLastIsIngProcessForObjective(byBitUser.IdTelegrame);
                if (LastIsIngProcessForObjective!=null &&  !LastIsIngProcessForObjective.EndDate.HasValue)
                {
                    
                    LastIsIngProcessForObjective.EndDate = DateTime.Now;
                    await _monitoringCoinWalletBalanceObjectiveProcessService.Update(LastIsIngProcessForObjective);
                }
            }

            if (DateTime.Now.Hour >= 5 && DateTime.Now.Hour < 7 )
            {
                return true;
            }
                foreach (var item in allsymbol)
            {
                var positionBybitInfo = await _byBitApiService.GetPositionInfo(new GetPositionInfo()
                {
                    category = EnumConverter.GetString(Category.Linear),
                    symbol = item,
                    apiKey = byBitUser.ApiKey,
                    secretKey = byBitUser.SecretKey
                });

                //no se deveria eliminar la posicion de bybit pero si de BD
                //pk ahy ke verifivar ke no ai orden avierta, en el caso ke no ahy orden avierta
                //se abre una para buscar confirmacion si no la tiene, lo ke si ahy ke eliminarla de la BD

                if (

                   ( positionBybitInfo.list != null &&
                    !positionBybitInfo.list.FirstOrDefault().side.Equals(EnumConverter.GetString(OrderSide.None))
                    && positionBybitInfo.list.Any() ) &&    ( !binanceOrderBD.ContainsKey(positionBybitInfo.list.FirstOrDefault().symbol)
                    || !IsIngProcessForObjective

                    )


                    /*&&
                    (!positionBybitInfo.list.FirstOrDefault().stopLoss.HasValue || positionBybitInfo.list.FirstOrDefault().stopLoss == 0  )*/
                    )
                {

                    //POSITION Satellite
                    Console.WriteLine("testok");

                    var tickersCategorySymbol = await _byBitApiService.GetTickersCategorySymbol(new GetTickers() { category = "inverse", symbol = positionBybitInfo.list.FirstOrDefault().symbol });


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
                    placeOrder.apiKey = byBitUser.ApiKey;
                    placeOrder.secretKey = byBitUser.SecretKey;
                    placeOrder.timeInForce = EnumConverter.GetString(TimeInForce.GoodTillCanceled);
                    placeOrder.reduceOnly = true;
                    placeOrder.closeOnTrigger = true;
                    placeOrder.qty = 0;//itemBinanceOrderBD.Amount;
                    placeOrder.orderType = OrderType.Market.ToString(); //OrderType.Limit.ToString();

                    if (positionBybitInfo.list.FirstOrDefault().side.Equals(EnumConverter.GetString(OrderSide.Buy)))
                    {
                        //placeOrder.price = positionBybitInfo.list.FirstOrDefault().markPrice - (positionBybitInfo.list.FirstOrDefault().markPrice * getInstrumentsInfo[placeOrder.symbol].priceFilter.tickSize);
                        placeOrder.price = positionBybitInfo.list.FirstOrDefault().markPrice;

                    }

                    if (positionBybitInfo.list.FirstOrDefault().side.Equals(EnumConverter.GetString(OrderSide.Sell)))
                    {
                        //placeOrder.price = positionBybitInfo.list.FirstOrDefault().markPrice + (positionBybitInfo.list.FirstOrDefault().markPrice * getInstrumentsInfo[placeOrder.symbol].priceFilter.tickSize);
                        placeOrder.price = positionBybitInfo.list.FirstOrDefault().markPrice;
                    }


                    if (positionBybitInfo.list.FirstOrDefault().side.Equals(EnumConverter.GetString(OrderSide.None)))
                    {
                        placeOrder.price = positionBybitInfo.list.FirstOrDefault().markPrice;
                    }

                    //tiene stopLoss y es satelite la cierro en  markPrice , pero se puede verificar y calcular un stopLoss mas obtimo
                    if ((positionBybitInfo.list.FirstOrDefault().stopLoss.HasValue || positionBybitInfo.list.FirstOrDefault().stopLoss != 0))
                    {
                       // placeOrder.price = positionBybitInfo.list.FirstOrDefault().markPrice;

                    }

                    if (placeOrder.price!=null)
                    {
                        if (tickersCategorySymbol.result.list.FirstOrDefault().lastPrice.Value.ToString(CultureInfo.InvariantCulture).Contains("."))
                        {
                            placeOrder.price = Math.Round(placeOrder.price.Value, tickersCategorySymbol.result.list.FirstOrDefault().lastPrice.Value.ToString(CultureInfo.InvariantCulture).Split('.')[1].Length, MidpointRounding.ToEven);

                        }
                        else
                        {
                            placeOrder.price = Math.Round(placeOrder.price.Value, 0, MidpointRounding.ToEven);
                        }

                        //await _byBitApiService.SetTradingStopForPosition(positionBybitInfo.list.FirstOrDefault(), getInstrumentsInfo[positionBybitInfo.list.FirstOrDefault().symbol], byBitUser);
                        var placeOrderByBit = await _byBitApiService.PlaceOrder(placeOrder);

                        File.AppendAllLines("ValidationCleanPositionHandlerlog.txt", new[] { string.Concat("deted from bybit symbol(UpdatePositionSatellite) :", placeOrder.symbol, "*******position deleted*********", DateTime.Now.ToString("yyyy/MM/dd HH:mm:ss:fff")) });
                        File.AppendAllLines("ValidationCleanPositionHandlerlog.txt", new[] { string.Concat(" placeOrder.side(UpdatePositionSatellite)  => ", placeOrder.side) });

                    }

                    
                }
            }
            return true;
        }
    }
}
