using System.Globalization;
using System.IO;
using System.Reflection;
using AutoMapper;
using CleanArchitecture.Application.Contracts.Infrastructure;
using CleanArchitecture.Application.Contracts.Persistence;
using CleanArchitecture.Application.Converters;
using CleanArchitecture.Application.Models;
using CleanArchitecture.Application.Models.Identity;
using CleanArchitecture.Domain.Binance;
using CleanArchitecture.Domain.Enum;
using MediatR;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using Microsoft.ML;
using Newtonsoft.Json;

namespace CleanArchitecture.Application.Features.Binance.Queries.LoadPosition
{
    public class LoadPositionHandler : IRequestHandler<LoadPositionQuery, int>
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
        private readonly IBinanceTraderPerformanceService _loadTraderPerformanceService;
        private readonly IBinanceMonitoringCoinWalletBalanceObjectiveProcessService _monitoringCoinWalletBalanceObjectiveProcessService;
        public readonly BinanceBybitSettings _binanceBybitSettings;
        private readonly JwtSettings _jwtSettings;
        private readonly IBinanceCacheByBitSymbolService _binanceCacheByBitSymbolService;
        public IConfiguration _configuration { get; }


        public LoadPositionHandler(IOptions<JwtSettings> jwtSettings, IOptions<BinanceBybitSettings> binanceBybitSettings, IUnitOfWork unitOfWork,  ITelegrameBotService telegrameBotService,
            IDonetZipService donetZipService, IFileService fileService, IByBitApiService byBitService,
            IBinanceTraderService binanceTraderService, IBinanceOrderService binanceOrderService,
            IBinanceByBitUsersService binanceByBitUsersService,
            IBinanceByBitOrderService binanceByBitOrderService,
            IBinanceMonitoringProcessService binanceMonitoringProcessService,
            IBinanceTraderPerformanceService loadTraderPerformanceService,
            IBinanceMonitoringCoinWalletBalanceObjectiveProcessService monitoringCoinWalletBalanceObjectiveProcessService,
            IConfiguration configuration, IBinanceCacheByBitSymbolService binanceCacheByBitSymbolService)
        {
            //_videoRepository = videoRepository;
            _unitOfWork = unitOfWork;
            _binanceBybitSettings = binanceBybitSettings.Value;
            _jwtSettings = jwtSettings.Value;
            _monitoringCoinWalletBalanceObjectiveProcessService = monitoringCoinWalletBalanceObjectiveProcessService ?? throw new ArgumentException(nameof(monitoringCoinWalletBalanceObjectiveProcessService));
            _telegrameBotService = telegrameBotService ?? throw new ArgumentException(nameof(telegrameBotService));
            _donetZipService = donetZipService ?? throw new ArgumentException(nameof(donetZipService));
            _fileService = fileService ?? throw new ArgumentException(nameof(fileService));
            _byBitApiService = byBitService ?? throw new ArgumentException(nameof(byBitService));
            _binanceTraderService = binanceTraderService ?? throw new ArgumentException(nameof(binanceTraderService));
            _binanceOrderService = binanceOrderService ?? throw new ArgumentException(nameof(binanceOrderService));
            _binanceByBitUsersService = binanceByBitUsersService ?? throw new ArgumentException(nameof(binanceByBitUsersService));
            _binanceByBitOrderService = binanceByBitOrderService ?? throw new ArgumentException(nameof(binanceByBitOrderService));
            _binanceMonitoringProcessService = binanceMonitoringProcessService ?? throw new ArgumentException(nameof(binanceMonitoringProcessService));
            _loadTraderPerformanceService = loadTraderPerformanceService ?? throw new ArgumentException(nameof(loadTraderPerformanceService));
            _configuration = configuration ?? throw new ArgumentException(nameof(configuration));
            _binanceCacheByBitSymbolService = binanceCacheByBitSymbolService ?? throw new ArgumentException(nameof(binanceCacheByBitSymbolService));
        }

        public async Task<int> Handle(LoadPositionQuery request, CancellationToken cancellationToken)
        {
            Console.WriteLine(_binanceBybitSettings.BinanceOrderqty);
            Console.WriteLine(_jwtSettings.Key);

            

            const double MinRoe = -30.0; // ROE mínimo permitido (-30%)
            const double MaxRoe = 10.0;  // ROE máximo permitido (10%)
            const double MinAmount = 300.0; // Tamaño mínimo de la posición


            Dictionary<string, BinanceTrader> dicBinanceTrader = (await _binanceTraderService.GetAll()).GroupBy(x => x.EncryptedUid).ToDictionary(p => p.Key, p => p.FirstOrDefault());
            /*TraderDataPerformanceBinance itemFirstDataPerformance = (await _loadTraderPerformanceService.GetAllTraderDataPerformanceBinanceForModel(dicBinanceTrader)).OrderBy(p=>p.CreatedOn).FirstOrDefault();
            if (itemFirstDataPerformance!=null)
            {
                request.DateBeginForLoad = DateTime.Parse( itemFirstDataPerformance.CreatedOn);
            }*/
            //request.DateBeginForLoad = new DateTime(2024, 04, 01, 23, 01, 1);
            

            var dicSymboleBD = (await _binanceOrderService.GetAll()).GroupBy(x => x.Symbol).ToDictionary(p => p.Key, p => p.FirstOrDefault().Symbol);
            // File.AppendAllLines("SYMBOLOAS_A_NO_IMPORTAR.txt", new[] { string.Concat(placeOrder.symbol) });
            var readNoIncludeSymbole = File.ReadAllLines("SYMBOLOAS_A_NO_IMPORTAR.txt").ToList().Distinct().ToDictionary(p=>p,p=>p);
            
            /*
              var result = (await GetAll())
           .ToDictionary(p => p.EncryptedUid, p => p.EncryptedUid );
            return result;
             */

            //CHECK IF WE ARE IN PROCESS IF NOT THEN CREATE
            if (!(await _binanceMonitoringProcessService.GetIsIngProcess()))
            {
              request.BinanceMonitoringProces=   await _binanceMonitoringProcessService.Create(new BinanceMonitoringProcess()
                {
                    CreatedOn = DateTime.Now
                });
            }
            //load Position Binance
            List<PositionData> positionsData =await _telegrameBotService.LoadPosition(_pathDataJson);
            if (positionsData==null)
            {
                return 0;
            }
            
             Dictionary<string,string> allUidTrader = await _binanceTraderService.GetAllDictionary();

            //OBTENIR DES UTILISATEURS QUI ONT PAYÉ LA REDEVANCE D'ABONNEMENT
            var allUserActive =await _binanceByBitUsersService.GetAllIsactive();
            int totalSendTelegrame = 0;

            

            foreach (var itemUser in allUserActive)
            {
           



                //await _byBitApiService.GetCoinInfo(itemUser.ApiKey, itemUser.SecretKey);
                //await _byBitApiService.SetDepositAccount(itemUser.ApiKey, itemUser.SecretKey);

                //NOUS OBTENONS LE MONTANT D'ARGENT QUE VOUS AVEZ SUR VOTRE COMPTE BYBIT
                var walletBalance = await _byBitApiService.WalletBalance(itemUser.ApiKey, itemUser.SecretKey);
                while (walletBalance==null)
                {
                    Thread.Sleep(5000);
                    walletBalance = await _byBitApiService.WalletBalance(itemUser.ApiKey, itemUser.SecretKey);

                    // code block to be executed
                }

                
                var coinWalletBalance = await _byBitApiService.GetCoinFromWalletBalance(walletBalance);
                File.AppendAllLines("coinWalletBalanceunrealisedPnl.txt", new[] { coinWalletBalance.unrealisedPnl.Value.ToString() });
                var getInstrumentsInfo = (await _byBitApiService.GetInstrumentsInfo(new GetInstrumentsInfoQuery()
                {
                    apiKey = itemUser.ApiKey,
                    secretKey = itemUser.SecretKey,
                    category = EnumConverter.GetString(Category.Linear)
                })).result.list.GroupBy(x=>x.symbol).ToDictionary(p=>p.Key,p=>p.FirstOrDefault() ) ;


                //verifico que no he cumplido los objetivos de el dia 
                bool IsIngProcessForObjective = false;
                IsIngProcessForObjective = (await _monitoringCoinWalletBalanceObjectiveProcessService.IsIngProcessForObjective(itemUser.IdTelegrame, coinWalletBalance));

                List<double> totalAmount = new List<double>();
                double variationtotalAmount = (((coinWalletBalance.equity - totalAmount.Sum()) * int.Parse(_configuration.GetSection("BinanceBybitSettings:BinanceOrderqty").Value)) / 100).Value;

                if (walletBalance != null && IsIngProcessForObjective)
                {
                    request.DateBeginForLoad = await _monitoringCoinWalletBalanceObjectiveProcessService.GetInitDateLoadPosition(itemUser.IdTelegrame);
                  

                    //get all trader et regarder si no esta presente en bd para insertarlo , todo en un dictionario
                    foreach (var item in positionsData)
                    {
                        //Add new trader
                        if (!allUidTrader.ContainsKey(item.code))
                        {
                            await _binanceTraderService.Create(new Domain.Binance.BinanceTrader()
                            {
                                EncryptedUid = item.code,
                                NickName = item.nickName,
                                CreatedOn = DateTime.Now
                            });
                            allUidTrader.Add(item.code, item.code);
                        }
                            //send position bybit
                            if (item.data != null && item.data.otherPositionRetList != null)
                            {
                                foreach (var itemPosition in item.data.otherPositionRetList)
                                {

                                try
                                {
                                    //COMENTE PORQUE LAS POSICIONES CREADAS POR ORDENES NO SON GUARDADAS Y ESTO CREA LAS POSICIONES Y LAS ELIMINA 3 
                                    //O 4 VECES LO QUE HACE ES PERDER DINERO CON ESTE NUEVO ALGORITMO POR ESO LO VOY A COMENTAR
                                    // var hasOrderOpenForThisSymbol = await _byBitApiService.GetOpenOrders(new GetOpenOrders() { category = EnumConverter.GetString(Category.Linear), apiKey = itemUser.ApiKey, secretKey = itemUser.SecretKey, symbol = itemPosition.symbol });
                                   // bool hasOrderOpen = await _byBitApiService.GethasOrderOpenForThisSymbolAndCancel(hasOrderOpenForThisSymbol, itemUser.ApiKey, itemUser.SecretKey);

                                    if (!dicSymboleBD.ContainsKey(itemPosition.symbol) && !readNoIncludeSymbole.ContainsKey(itemPosition.symbol)) //&& !hasOrderOpen
                                    {
                                       

                                        if (itemPosition.amount < 0)//&& item.LongShort.ContainsKey(itemPosition.symbol)
                                        {
                                            itemPosition.side = EnumConverter.GetString(OrderSide.Sell);//item.LongShort[itemPosition.symbol].Trim();
                                        }

                                        else
                                        {
                                            itemPosition.side = EnumConverter.GetString(OrderSide.Buy);//EnumConverter.GetString(OrderSide.Buy);
                                        }

                                        //Price by symbol
                                        var tickersCategorySymbol = await _byBitApiService.GetTickersCategorySymbol(new GetTickers() { category = "inverse", symbol = itemPosition.symbol });


                                        //New Order UTC
                                        DateTime updateTime = new DateTime(itemPosition.updateTime[0], itemPosition.updateTime[1],
                                            itemPosition.updateTime[2], itemPosition.updateTime[3], itemPosition.updateTime[4], itemPosition.updateTime[5]);
                                
                                        DateTime dateTimeParis = DatetimeConvert.GetDateParisTimeZone(updateTime);

                                        bool isInCache = await _binanceCacheByBitSymbolService.HasSymbolInCache(itemPosition.symbol);
                                        double roePercentage = Math.Abs(itemPosition.roe.Value) * 100;
                                        bool isRoeInRange = true; // roePercentage <= MaxRoe; //roePercentage >= MinRoe &&


                                        if (!isInCache
                                         && (      ( ( request.DateBeginForLoad < dateTimeParis) )   )
                                         && Math.Abs(itemPosition.amount.Value) > MinAmount)//|| (request.DateBeginForLoad> updateTime && itemPosition.pnl<0)
                                        {
                                            var newOrder = new Domain.Binance.BinanceOrder()
                                            {
                                                Leverage = itemPosition.leverage,
                                                MarkPrice = itemPosition.markPrice,
                                                Amount = itemPosition.amount,
                                                EntryPrice = itemPosition.entryPrice,
                                                Pnl = itemPosition.pnl,
                                                TradeBefore = itemPosition.tradeBefore,
                                                CreatedOn = DateTime.Now,
                                                UpdateTime = dateTimeParis,
                                                Yellow = itemPosition.yellow,
                                                EncryptedUid = item.code,
                                                Symbol = itemPosition.symbol,
                                                Side = itemPosition.side,
                                                IsForClosed = false,
                                                BinanceByBitOrders = new List<BinanceByBitOrder>()
                                            };
                                            var getOrderIfNullIsValidationForCreateOrder = await _binanceOrderService.getOrderIfNullIsValidationForCreateOrder(newOrder, item.code);
                                            var resultOrderByUserIdTelegrame = await _binanceOrderService.GetOrderByUserIdTelegrame(newOrder, itemUser.IdTelegrame, item.code);
                                            if (resultOrderByUserIdTelegrame == null)
                                            {
                                               int binanceOrderqty = int.Parse( _configuration.GetSection("BinanceBybitSettings:BinanceOrderqty").Value);
                                                //nous créons la position broker  bybit
                                                PlaceOrder placeOrder = new PlaceOrder();
                                                placeOrder.symbol = itemPosition.symbol;
                                                placeOrder.orderType = OrderType.Market.ToString();
                                                placeOrder.side = itemPosition.side;
                                                placeOrder.category = EnumConverter.GetString(Category.Linear);//"linear";
                                                placeOrder.apiKey = itemUser.ApiKey;
                                                placeOrder.secretKey = itemUser.SecretKey;

                                                double lastPrice; 

                                                if (tickersCategorySymbol.result.list.Any())
                                                {
                                                    lastPrice = tickersCategorySymbol.result.list.FirstOrDefault().lastPrice.Value;

                                                    placeOrder.qty = Math.Round(((((coinWalletBalance.equity - totalAmount.Sum()) * binanceOrderqty) / 100).Value) / lastPrice, 2, MidpointRounding.ToEven);

                                                    if (placeOrder.qty.Value.ToString(CultureInfo.InvariantCulture).Split('.')[0].Length == 1)
                                                    {
                                                        placeOrder.qty = Math.Round(((((coinWalletBalance.equity - totalAmount.Sum()) * binanceOrderqty) / 100).Value) / lastPrice, 2, MidpointRounding.ToEven);
                                                    }

                                                    if (placeOrder.qty.Value.ToString(CultureInfo.InvariantCulture).Split('.')[0].Length == 2)
                                                    {
                                                        placeOrder.qty = Math.Round(((((coinWalletBalance.equity - totalAmount.Sum()) * binanceOrderqty) / 100).Value) / lastPrice, 1, MidpointRounding.ToEven);
                                                    }

                                                    if (placeOrder.qty.Value.ToString(CultureInfo.InvariantCulture).Split('.')[0].Length >= 4)
                                                    {
                                                        placeOrder.qty = Math.Round(((((coinWalletBalance.equity - totalAmount.Sum()) * binanceOrderqty) / 100).Value) / lastPrice, 0, MidpointRounding.ToEven);
                                                    }
                                                    string amountBinance = itemPosition.amount.Value.ToString(CultureInfo.InvariantCulture);

                                                    string minOrderQty = "0.1";     
                                                    if (getInstrumentsInfo.ContainsKey(placeOrder.symbol))
                                                    {
                                                        minOrderQty = getInstrumentsInfo[placeOrder.symbol].lotSizeFilter.minOrderQty.Value.ToString(CultureInfo.InvariantCulture);

                                                    }

                                                     

                                                    if (itemPosition.amount < 0)//&& item.LongShort.ContainsKey(itemPosition.symbol)
                                                    {
                                                        itemPosition.side = EnumConverter.GetString(OrderSide.Sell);//item.LongShort[itemPosition.symbol].Trim();
                                                    }


                                                    if (amountBinance.Contains("."))
                                                    {

                                                        if (minOrderQty.Contains('.') && getInstrumentsInfo.ContainsKey(placeOrder.symbol) )
                                                        {
                                                            placeOrder.qty = Math.Round(((((coinWalletBalance.equity - totalAmount.Sum()) * binanceOrderqty) / 100).Value) / lastPrice,
                                                           getInstrumentsInfo[placeOrder.symbol].lotSizeFilter.minOrderQty.Value.ToString(CultureInfo.InvariantCulture).Split('.')[1].Length
                                                           , MidpointRounding.ToEven);

                                                        }
                                                        else
                                                        {
                                                            placeOrder.qty = Math.Round(  ((((coinWalletBalance.equity - totalAmount.Sum()) * binanceOrderqty) / 100).Value) / lastPrice,
                                                           0
                                                           , MidpointRounding.ToEven);
                                                        }
                                                    }
                                                    else
                                                    {
                                                        placeOrder.qty = Math.Round(((( (coinWalletBalance.equity - totalAmount.Sum()) * binanceOrderqty) / 100).Value) / lastPrice, 0, MidpointRounding.ToEven);

                                                        if (placeOrder.qty==0)
                                                        {
                                                            if (minOrderQty.Contains('.') &&   getInstrumentsInfo.ContainsKey(placeOrder.symbol))
                                                            {
                                                                placeOrder.qty = Math.Round(((((coinWalletBalance.equity - totalAmount.Sum()) * binanceOrderqty) / 100).Value) / lastPrice,
                                                               getInstrumentsInfo[placeOrder.symbol].lotSizeFilter.minOrderQty.Value.ToString(CultureInfo.InvariantCulture).Split('.')[1].Length
                                                               , MidpointRounding.ToEven);

                                                            }
                                                            else
                                                            {
                                                                placeOrder.qty = Math.Round(((((coinWalletBalance.equity - totalAmount.Sum()) * binanceOrderqty) / 100).Value) / lastPrice,
                                                               0
                                                               , MidpointRounding.ToEven);
                                                            }

                                                        }
                                                    }
                                                    string qty = placeOrder.qty.Value.ToString(CultureInfo.InvariantCulture);
                                                    double stopLoss = (((lastPrice * 30) / 100)); //30
                                                    if (placeOrder.side.Equals(EnumConverter.GetString(OrderSide.Buy)))
                                                    {
                                                        stopLoss = lastPrice - stopLoss;    

                                                    }
                                                    if (placeOrder.side.Equals(EnumConverter.GetString(OrderSide.Sell)))
                                                    {
                                                        stopLoss = lastPrice + stopLoss;
                                                    }
                                                    //COMENTE PORQUE LAS POSICIONES CREADAS POR ORDENES NO SON GUARDADAS Y ESTO CREA LAS POSICIONES Y LAS ELIMINA 3 
                                                    //O 4 VECES LO QUE HACE ES PERDER DINERO CON ESTE NUEVO ALGORITMO POR ESO LO VOY A COMENTAR
                                                    //if (placeOrder.side.Equals(EnumConverter.GetString(OrderSide.Buy)))
                                                    //{
                                                    //    //placeOrder.price = lastPrice + (getInstrumentsInfo[placeOrder.symbol].priceFilter.tickSize * lastPrice);
                                                    //    placeOrder.price = lastPrice + (0.15 * lastPrice);
                                                    //}
                                                    //else
                                                    //{
                                                    //    //placeOrder.price = lastPrice - (getInstrumentsInfo[placeOrder.symbol].priceFilter.tickSize * lastPrice);
                                                    //    placeOrder.price = lastPrice - (0.15 * lastPrice);
                                                    //}


                                                    if (tickersCategorySymbol.result.list.FirstOrDefault().lastPrice.Value.ToString(CultureInfo.InvariantCulture).Contains("."))
                                                    {

                                                      placeOrder.stopLoss = Math.Round(stopLoss, tickersCategorySymbol.result.list.FirstOrDefault().lastPrice.Value.ToString(CultureInfo.InvariantCulture).Split('.')[1].Length, MidpointRounding.ToEven);
                                                        //placeOrder.price = Math.Round(placeOrder.price.Value, tickersCategorySymbol.result.list.FirstOrDefault().lastPrice.Value.ToString(CultureInfo.InvariantCulture).Split('.')[1].Length, MidpointRounding.ToEven);
                                                    }
                                                    else
                                                    {//tickersCategorySymbol.result.list.FirstOrDefault().lastPrice.Value
                                                        placeOrder.stopLoss = Math.Round(stopLoss, 0, MidpointRounding.ToEven);
                                                     //placeOrder.price = Math.Round(placeOrder.price.Value, 0, MidpointRounding.ToEven);
                                                    }
                                                }
                                                else
                                                {
                                                    placeOrder.qty = 0;
                                                }

                                                if (getInstrumentsInfo.ContainsKey(placeOrder.symbol))
                                                {
                                                    if (placeOrder.qty > getInstrumentsInfo[placeOrder.symbol].lotSizeFilter.maxOrderQty)
                                                    {
                                                        placeOrder.qty = getInstrumentsInfo[placeOrder.symbol].lotSizeFilter.maxOrderQty;
                                                    }

                                                    if (placeOrder.qty < getInstrumentsInfo[placeOrder.symbol].lotSizeFilter.minOrderQty)
                                                    {
                                                        placeOrder.qty = getInstrumentsInfo[placeOrder.symbol].lotSizeFilter.minOrderQty;
                                                    }
                                                }


                                                //placeOrder.timeInForce = EnumConverter.GetString(TimeInForce.PostOnly);
                                                RootOrderReponse createPositionByBit = await _byBitApiService.PlaceOrder(placeOrder);
                                                bool setLeverage = false;

                                                if (createPositionByBit.result.orderId != null)
                                                {
                                                    dicSymboleBD.Add(itemPosition.symbol, itemPosition.symbol);
                                                    totalAmount.Add(variationtotalAmount);
                                                    variationtotalAmount = (((coinWalletBalance.equity - totalAmount.Sum()) * binanceOrderqty) / 100).Value;
                                                }
                                                else
                                                {
                                                    //createPositionByBit.result.
                                                    File.AppendAllLines("WriteLinesLogPositionNotAdd.txt", new[] { string.Concat("amount :", variationtotalAmount, "*******position not add*********", DateTime.Now.ToString("yyyy/MM/dd HH:mm:ss:fff")) });
                                                    File.AppendAllLines("WriteLinesLogPositionNotAdd.txt", new[] { string.Concat("retCode  => ", createPositionByBit.retCode,
                                                " retMsg  => ", createPositionByBit.retMsg," symbol  => ", placeOrder.symbol) });

                                                }

                                                if (itemUser.IdTelegrame == 1476318624 && createPositionByBit.result.orderId != null)
                                                {

                                                    int intSetLeverage;
                                                    bool success = int.TryParse(itemPosition.leverage.ToString(), out intSetLeverage);

                                                    //intSetLeverage = 5; 
                                                    if (intSetLeverage > int.Parse(_configuration.GetSection("BinanceBybitSettings:Leverage").Value))
                                                    {
                                                        intSetLeverage = int.Parse(_configuration.GetSection("BinanceBybitSettings:Leverage").Value);
                                                    }

                                                    setLeverage = await _byBitApiService.SetLeverage(new SetLeverage()
                                                    {
                                                        ApiKey = itemUser.ApiKey,
                                                        SecretKey = itemUser.SecretKey,
                                                        Symbol = itemPosition.symbol,
                                                        Category = EnumConverter.GetString(Category.Linear),//"linear",
                                                        BuyLeverage = intSetLeverage.ToString(),
                                                        SellLeverage = intSetLeverage.ToString()
                                                    });
                                                }
                                                else
                                                {
                                                    if (createPositionByBit.result.orderId != null)
                                                    {
                                                        setLeverage = await _byBitApiService.SetLeverage(new SetLeverage()
                                                        {
                                                            ApiKey = itemUser.ApiKey,
                                                            SecretKey = itemUser.SecretKey,
                                                            Symbol = itemPosition.symbol,
                                                            Category = EnumConverter.GetString(Category.Linear),//"linear",
                                                            BuyLeverage = "30",
                                                            SellLeverage = "30"
                                                        });
                                                    }
                                                }

                                                //set Isolated Margin

                                                //tengo ke validar correctamente isValidationForCreateOrder  porque si la posicion existe no devo volverla a crear
                                                //solo tengo ke agregarle un itel  a BinanceByBitOrders   porque si no tendre  doblones todo el tiempo
                                                //es una validacion a aser correctamente ver la funcion que envia el boolean isValidationForCreateOrder
                                                if (getOrderIfNullIsValidationForCreateOrder == null && createPositionByBit.result.orderId != null)
                                                {


                                                    try
                                                    {
                                                        //NOUS ENVOYONS LA POSITION AU GRUP TELEGRAME SI NOUS MODIFIONS LA POSITION
                                                        bool resultSendTelegrame = await _telegrameBotService.SendOtherPositionRetList(itemPosition, itemUser.IdTelegrame,
                                                            new Dictionary<string, string> (), item.nickName);
                                                        totalSendTelegrame++;
                                                    }
                                                    catch (Exception ex)
                                                    {
                                                        Console.WriteLine("***********error telegrame************");
                                                        Console.WriteLine(ex.Message);
                                                    }


                                                    if (createPositionByBit.result.orderId != null)
                                                    {
                                                        //NOUS AJOUTONS LA POSITION DANS LA BASE DE DONNÉES
                                                        newOrder.BinanceByBitOrders.Add(new BinanceByBitOrder()
                                                        {
                                                            ByBitOrderId = createPositionByBit.result.orderId,
                                                            IdTelegrame = itemUser.IdTelegrame,
                                                            ByBitOrderLinkId = createPositionByBit.result.orderLinkId,
                                                            CreatedOn = DateTime.Now,
                                                            Amount = totalAmount.LastOrDefault()
                                                        });
                                                        await _binanceOrderService.Create(newOrder);
                                                    }
                                                }
                                                else
                                                {

                                                    if (createPositionByBit.result.orderId != null)
                                                    {

                                                        bool resultSendTelegrame = await _telegrameBotService.SendDataPositions(item, itemUser.IdTelegrame);
                                                        totalSendTelegrame++;

                                                        await _binanceByBitOrderService.Create(new BinanceByBitOrder()
                                                        {
                                                            BinanceOrderId = getOrderIfNullIsValidationForCreateOrder.Id,
                                                            ByBitOrderId = createPositionByBit.result.orderId,
                                                            IdTelegrame = itemUser.IdTelegrame,
                                                            ByBitOrderLinkId = createPositionByBit.result.orderLinkId,
                                                            CreatedOn = DateTime.Now,
                                                            Amount =  placeOrder.qty 
                                                        });
                                                    }


                                                }
                                            }

                                        }
                                    }


                                }
                                catch (Exception e)
                                {
                                    Console.WriteLine(e.Message);
                                }

                            }

                            

                        }
                       


                        //delete doublon dans Binance_ByBit_Users et add orderForDeleted
                        //orderForDeleted.AddRange( await _binanceOrderService.GetBinanceByBitOrdersForByEncryptedUid(item.code));
                    }
                }
            }
         
            
            //zip position
            await _donetZipService.ZipFolderPosition(_pathDataJson, _pathDataJsonZip);
            await _fileService.DeleteAllFiles(_pathDataJson);

            //UPDATE IN PROCESS IF NOT THEN CREATE
            // var binanceMonitoringProcess = await _binanceMonitoringProcessService.GetLatBinanceMonitoringProcess();
            request.BinanceMonitoringProces.EndDate = DateTime.Now;
             await _binanceMonitoringProcessService.Update(request.BinanceMonitoringProces);
            return positionsData.Count;
        }
    }
}
