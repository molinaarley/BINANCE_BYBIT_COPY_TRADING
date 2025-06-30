
using System.Collections.Concurrent;
using System.Collections.Generic;
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
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;

namespace CleanArchitecture.Application.Features.Binance.Queries.CleanPosition
{
    public class CleanPositionHandler : IRequestHandler<CleanPositionQuery, bool>
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
        private readonly IBinanceTraderUrlForUpdatePositionBinanceQueryService _binanceTraderUrlForUpdatePositionBinanceQueryService;
        public IConfiguration _configuration { get; }


        public CleanPositionHandler(IUnitOfWork unitOfWork, ITelegrameBotService telegrameBotService,
            IDonetZipService donetZipService, IFileService fileService, IByBitApiService byBitService,
            IBinanceTraderService binanceTraderService, IBinanceOrderService binanceOrderService,
            IBinanceByBitUsersService binanceByBitUsersService,
            IBinanceByBitOrderService binanceByBitOrderService,
            IBinanceMonitoringProcessService binanceMonitoringProcessService,
            IBinanceTraderUrlForUpdatePositionBinanceQueryService binanceTraderUrlForUpdatePositionBinanceQueryService,
            IConfiguration configuration)
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
            _binanceTraderUrlForUpdatePositionBinanceQueryService = binanceTraderUrlForUpdatePositionBinanceQueryService ?? throw new ArgumentException(nameof(binanceTraderUrlForUpdatePositionBinanceQueryService));
            _configuration = configuration ?? throw new ArgumentException(nameof(configuration));
        }

     
            public async Task<bool> Handle(CleanPositionQuery request, CancellationToken cancellationToken)
        {

            //CHECK IF WE ARE IN PROCESS IF NOT THEN CREATE
            /*BinanceMonitoringProcess resultBinanceMonitoringProcess = new BinanceMonitoringProcess();
            if ( !(await _binanceMonitoringProcessService.GetIsIngProcess()  ))
            {
                resultBinanceMonitoringProcess= await _binanceMonitoringProcessService.Create( new BinanceMonitoringProcess()
                {
                    CreatedOn = DateTime.Now
                } );
            }*/
 
            //load Position Binance
            List<PositionData> positionsData = await _telegrameBotService.LoadPosition(_pathDataJson);
            if (positionsData==null)
            {
                return false;
            }
            Dictionary<string, string> allUidTrader = await _binanceTraderService.GetAllDictionary();

            //OBTENIR DES UTILISATEURS QUI ONT PAYÉ L'ABONNEMENT
            var allUserActive = await _binanceByBitUsersService.GetAllIsactive();
         
         
            List<OtherPositionRetList> allotherPositionRetList = new List<OtherPositionRetList>();

            ConcurrentDictionary<string, List<OtherPositionRetList>> dicallOtherPositionRetList = new ConcurrentDictionary<string, List<OtherPositionRetList>>();

            //var allByBitOrderIndDic = await _binanceByBitOrderService.GetAllInDictionary();
            var allBinanceOrder = await _binanceOrderService.GetAll(); 
          //Lsit Position by tredeur
            foreach (var item in positionsData)
            {
                //send position bybit

                if (item.data != null && item.data.otherPositionRetList != null)
                {

                    if (!dicallOtherPositionRetList.ContainsKey(item.code))
                    {
                        dicallOtherPositionRetList.TryAdd(item.code, new List<OtherPositionRetList>());
                        dicallOtherPositionRetList[item.code].AddRange(item.data.otherPositionRetList);
                    }
                    else
                    {
                        dicallOtherPositionRetList[item.code].AddRange(item.data.otherPositionRetList);
                    }

                    allotherPositionRetList.AddRange(item.data.otherPositionRetList);
                }
            }
            //clean position
                foreach (var itemBinanceOrderBD in allBinanceOrder)
            {



                
                    if (dicallOtherPositionRetList.ContainsKey(itemBinanceOrderBD.EncryptedUid))
                    {
                        var resultBinanceOrder = dicallOtherPositionRetList[itemBinanceOrderBD.EncryptedUid].
                            Where(p => p.symbol.Equals(itemBinanceOrderBD.Symbol, StringComparison.InvariantCulture)
                       && p.side.Equals(itemBinanceOrderBD.Side, StringComparison.InvariantCulture) //&& p.entryPrice == itemBinanceOrderBD.EntryPrice
                                                                                                    //&& dicallOtherPositionRetList[itemBinanceOrder.EncryptedUid].  
                       ).FirstOrDefault();



                        if (resultBinanceOrder == null)
                        { //no se deveria eliminar la posicion de bybit pero si de BD
                          //pk ahy ke verifivar ke no ai orden avierta, en el caso ke no ahy orden avierta
                          //se abre una para buscar confirmacion si no la tiene, lo ke si ahy ke eliminarla de la BD
                            await DeletedPosition(allUserActive, itemBinanceOrderBD);
                        }
                        else
                        {
                            await DeletedOrderDBOrSwitchCrossIsolatedMargin(allUserActive, itemBinanceOrderBD);
                        }
                    }
                    else
                    {

                        var result = await _binanceTraderUrlForUpdatePositionBinanceQueryService.GetTraderUrlForUpdatePositionBinance();
                        var totalTraderUrlForUpdatePositionBinanceQuery = result.Select(p => p.EncryptedUid).Take(int.Parse(_configuration.GetSection("BinanceBybitSettings:TotalTraderUrlForUpdatePositionBinanceQuery").Value)).ToList();

                        if (!totalTraderUrlForUpdatePositionBinanceQuery.Contains(itemBinanceOrderBD.EncryptedUid))
                        {
                            File.AppendAllLines("AllSupresion.txt", new[] { string.Concat(" AllSupresion (Handle) =>date: ", DateTime.Now.ToString(), " guid:  ", itemBinanceOrderBD.EncryptedUid) });
                            foreach (var itemUser in allUserActive)
                            {
                                foreach (var itemOritemBinanceByBitOrders in itemBinanceOrderBD.BinanceByBitOrders)
                                {

                                    //Task<GetPositionInfoResult> GetPositionInfo(GetPositionInfo getPositionInfo)
                                    var positionBybitInfo = await _byBitApiService.GetPositionInfo(new GetPositionInfo()
                                    {
                                        category = "linear",
                                        symbol = itemBinanceOrderBD.Symbol,
                                        apiKey = itemUser.ApiKey,
                                        secretKey = itemUser.SecretKey
                                    });


                                    RootTickersCategorySymbol tickersCategorySymbol = new RootTickersCategorySymbol();

                                    if (positionBybitInfo != null && positionBybitInfo.list != null && positionBybitInfo.list.Any())
                                    {
                                        tickersCategorySymbol = await _byBitApiService.GetTickersCategorySymbol(new GetTickers() { category = "inverse", symbol = positionBybitInfo.list.FirstOrDefault().symbol });
                                    }


                                    //nous créons la position broker  bybit
                                    if (positionBybitInfo != null && positionBybitInfo.list != null && positionBybitInfo.list.Any())
                                    {

                                        var getInstrumentsInfo = (await _byBitApiService.GetInstrumentsInfo(new GetInstrumentsInfoQuery()
                                        {
                                            apiKey = itemUser.ApiKey,
                                            secretKey = itemUser.SecretKey,
                                            category = EnumConverter.GetString(Category.Linear)
                                        })).result.list.GroupBy(x => x.symbol).ToDictionary(p => p.Key, p => p.FirstOrDefault());


                                        PlaceOrder placeOrder = new PlaceOrder();
                                        placeOrder.symbol = positionBybitInfo.list.FirstOrDefault().symbol;//itemPosition.symbol;
                                        placeOrder.orderType = OrderType.Market.ToString();


                                        if (positionBybitInfo.list.FirstOrDefault().side.Equals(EnumConverter.GetString(OrderSide.Buy)))
                                        {
                                            placeOrder.side = "Sell";//EnumConverter.GetString(OrderSide.Sell);
                                        }

                                        if (positionBybitInfo.list.FirstOrDefault().side.Equals(EnumConverter.GetString(OrderSide.Sell)))
                                        {
                                            placeOrder.side = "Long";

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
                                            //await _byBitApiService.SetTradingStopForPosition(positionBybitInfo.list.FirstOrDefault(), getInstrumentsInfo[positionBybitInfo.list.FirstOrDefault().symbol], itemUser);
                                            var placeOrderByBit = await _byBitApiService.PlaceOrder(placeOrder);
                                        }




                                    }
                                    await _binanceByBitOrderService.Deleted(itemOritemBinanceByBitOrders);
                                    File.AppendAllLines("CleanPositionHandler.txt", new[] { string.Concat(" deleted  symbol/guid : ", itemBinanceOrderBD.Symbol, "#", itemBinanceOrderBD.EncryptedUid) });

                                }
                            }
                            await _binanceOrderService.DeletedOrder(itemBinanceOrderBD);


                        }




                    }
       
            }
            return true;
        }



        private async Task<bool> DeletedPosition(List<BinanceByBitUser> allUserActive, BinanceOrder itemBinanceOrderBD)
        {

            foreach (var itemUser in allUserActive)
            {
                foreach (var itemOritemBinanceByBitOrders in itemBinanceOrderBD.BinanceByBitOrders)
                {
                    //Task<GetPositionInfoResult> GetPositionInfo(GetPositionInfo getPositionInfo)
                    var positionBybitInfo = await _byBitApiService.GetPositionInfo(new GetPositionInfo()
                    {
                        category = "linear",
                        symbol = itemBinanceOrderBD.Symbol,
                        apiKey = itemUser.ApiKey,
                        secretKey = itemUser.SecretKey
                    });

                    //n est pas en BD  don il faut eliminer la position
                    if (positionBybitInfo != null && positionBybitInfo.list != null && positionBybitInfo.list.Any())
                    {
                        var getInstrumentsInfo = (await _byBitApiService.GetInstrumentsInfo(new GetInstrumentsInfoQuery()
                        {
                            apiKey = itemUser.ApiKey,
                            secretKey = itemUser.SecretKey,
                            category = EnumConverter.GetString(Category.Linear)
                        })).result.list.GroupBy(x => x.symbol).ToDictionary(p => p.Key, p => p.FirstOrDefault());



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
                        placeOrder.apiKey = itemUser.ApiKey;
                        placeOrder.secretKey = itemUser.SecretKey;
                        placeOrder.timeInForce = EnumConverter.GetString(TimeInForce.GoodTillCanceled);
                        placeOrder.reduceOnly = true;
                        placeOrder.closeOnTrigger = true;
                        placeOrder.qty = 0;//itemBinanceOrderBD.Amount;
                        placeOrder.orderType = OrderType.Limit.ToString();


                        if (positionBybitInfo.list.FirstOrDefault().side.Equals(EnumConverter.GetString(OrderSide.Buy)) && getInstrumentsInfo.ContainsKey(positionBybitInfo.list.FirstOrDefault().symbol))
                        {
                            placeOrder.price = positionBybitInfo.list.FirstOrDefault().markPrice - (positionBybitInfo.list.FirstOrDefault().markPrice * getInstrumentsInfo[positionBybitInfo.list.FirstOrDefault().symbol].priceFilter.tickSize);

                        }

                        if (positionBybitInfo.list.FirstOrDefault().side.Equals(EnumConverter.GetString(OrderSide.Sell)) &&  getInstrumentsInfo.ContainsKey(positionBybitInfo.list.FirstOrDefault().symbol))
                        {
                            placeOrder.price = positionBybitInfo.list.FirstOrDefault().markPrice + (positionBybitInfo.list.FirstOrDefault().markPrice * getInstrumentsInfo[positionBybitInfo.list.FirstOrDefault().symbol].priceFilter.tickSize);
                        }

                        if (positionBybitInfo.list.FirstOrDefault().side.Equals(EnumConverter.GetString(OrderSide.None)))
                        {
                            placeOrder.price = positionBybitInfo.list.FirstOrDefault().markPrice;
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
                            //await _byBitApiService.SetTradingStopForPosition(positionBybitInfo.list.FirstOrDefault(), getInstrumentsInfo[positionBybitInfo.list.FirstOrDefault().symbol],itemUser);
                            var placeOrderByBit = await _byBitApiService.PlaceOrder(placeOrder);

                        }




                    }
                    await _binanceByBitOrderService.Deleted(itemOritemBinanceByBitOrders);
                    File.AppendAllLines("DeletedPositionCleanPositionHandler.txt", new[] { string.Concat(" deleted  symbol/guid : ", itemBinanceOrderBD.Symbol, "#", itemBinanceOrderBD.EncryptedUid) });
                }
            }
            await _binanceOrderService.DeletedOrder(itemBinanceOrderBD);
            return true;
        }
        
            private async Task<bool> DeletedOrderDBOrSwitchCrossIsolatedMargin(List<BinanceByBitUser> allUserActive, BinanceOrder itemBinanceOrderBD)
        {
            foreach (var itemUser in allUserActive)
            {
                //var hasOrderOpenForThisSymbol = await _byBitApiService.GetOpenOrders(new GetOpenOrders() { category = EnumConverter.GetString(Category.Linear), apiKey = itemUser.ApiKey, secretKey = itemUser.SecretKey, symbol = itemBinanceOrderBD.Symbol });
                //bool hasOrderOpen = await _byBitApiService.GethasOrderOpenForThisSymbolAndCancel(hasOrderOpenForThisSymbol, itemUser.ApiKey, itemUser.SecretKey);

                var positionBybitInfo = await _byBitApiService.GetPositionInfo(new GetPositionInfo()
                {
                    category = "linear",
                    symbol = itemBinanceOrderBD.Symbol,
                    apiKey = itemUser.ApiKey,
                    secretKey = itemUser.SecretKey
                });
                
                if (positionBybitInfo.list.FirstOrDefault().side.Equals(EnumConverter.GetString(OrderSide.None)))
                {
                    var itemByBitForDelted = itemBinanceOrderBD.BinanceByBitOrders.Where(p => p.IdTelegrame == itemUser.IdTelegrame).FirstOrDefault();

                    if (itemByBitForDelted != null)
                    {
                        await _binanceByBitOrderService.Deleted(itemByBitForDelted);
                        if (!itemBinanceOrderBD.BinanceByBitOrders.Any())
                        {
                            await _binanceOrderService.DeletedOrder(itemBinanceOrderBD);
                        }
                    }
                }
                else
                {   //este codigo es innecesario  porque ya por defecto las posiciones se abren en isolat eso es una configuracion que se hace en bybit
                    /*if (positionBybitInfo.list.FirstOrDefault().tradeMode == 0)
                    {
                        var switchCrossIsolatedMargin = await _byBitApiService.SwitchCrossIsolatedMargin(new SwitchIsolated()
                        {
                            ApiKey = itemUser.ApiKey,
                            SecretKey = itemUser.SecretKey,
                            Symbol = positionBybitInfo.list.FirstOrDefault().symbol,
                            Category = EnumConverter.GetString(Category.Linear),//"linear",
                            BuyLeverage = positionBybitInfo.list.FirstOrDefault().leverage.ToString(),
                            SellLeverage = positionBybitInfo.list.FirstOrDefault().leverage.ToString(),
                            TradeMode = 1
                        });

                    }*/
                }
            }
            return true;
        }


    }
}
