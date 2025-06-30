using System.Globalization;
using System.IO;
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








        public LoadPositionHandler(IUnitOfWork unitOfWork,  ITelegrameBotService telegrameBotService,
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

        public async Task<int> Handle(LoadPositionQuery request, CancellationToken cancellationToken)
        {
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
                 await _binanceMonitoringProcessService.Create(new BinanceMonitoringProcess()
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

                /*
                  var dicFileEntriesType = fileEntriesType.GroupBy(x => x)
               .ToDictionary(p => new FileInfo(p.Key).Name.Split('_')[0], p => new FileInfo(p.FirstOrDefault()));
                 */

                List<double> totalAmount = new List<double>();
                double variationtotalAmount = (((coinWalletBalance.equity - totalAmount.Sum()) * 30) / 100).Value;

                if (walletBalance != null)
                {
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
                                if (!dicSymboleBD.ContainsKey(itemPosition.symbol)  && !readNoIncludeSymbole.ContainsKey(itemPosition.symbol))
                                {

                                    if (item.LongShort.ContainsKey(itemPosition.symbol))
                                    {
                                        itemPosition.side = item.LongShort[itemPosition.symbol].Trim();
                                    }
                                    else
                                    {
                                        itemPosition.side = EnumConverter.GetString(OrderSide.Buy);
                                    }

                                    var tickersCategorySymbol = await _byBitApiService.GetTickersCategorySymbol("inverse", itemPosition.symbol);


                                    //New Order
                                    DateTime updateTime = new DateTime(itemPosition.updateTime[0], itemPosition.updateTime[1],
                                        itemPosition.updateTime[2], itemPosition.updateTime[3], itemPosition.updateTime[4], itemPosition.updateTime[5]);

                                    if (request.DateBeginForLoad< updateTime  
                                      )//|| (request.DateBeginForLoad> updateTime && itemPosition.pnl<0)
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
                                            UpdateTime = updateTime,
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


                                                //totalAmount.Add(  ((coinWalletBalance.equity * 30) / 100).Value );


                                                placeOrder.qty = Math.Round(((((coinWalletBalance.equity - totalAmount.Sum()) * 3) / 100).Value) / lastPrice, 2, MidpointRounding.ToEven);

                                                if (placeOrder.qty.Value.ToString(CultureInfo.InvariantCulture).Split('.')[0].Length == 1)
                                                {
                                                    placeOrder.qty = Math.Round(((((coinWalletBalance.equity - totalAmount.Sum()) * 3) / 100).Value) / lastPrice, 2, MidpointRounding.ToEven);
                                                }

                                                if (placeOrder.qty.Value.ToString(CultureInfo.InvariantCulture).Split('.')[0].Length == 2)
                                                {
                                                    placeOrder.qty = Math.Round(((((coinWalletBalance.equity - totalAmount.Sum()) * 3) / 100).Value) / lastPrice, 1, MidpointRounding.ToEven);
                                                }


                                                if (placeOrder.qty.Value.ToString(CultureInfo.InvariantCulture).Split('.')[0].Length >= 4)
                                                {
                                                    placeOrder.qty = Math.Round(((((coinWalletBalance.equity - totalAmount.Sum()) * 3) / 100).Value) / lastPrice, 0, MidpointRounding.ToEven);
                                                }


                                                string amountBinance = itemPosition.amount.Value.ToString(CultureInfo.InvariantCulture);


                                                if (amountBinance.Contains("."))
                                                {
                                                    string minOrderQty = getInstrumentsInfo[placeOrder.symbol].lotSizeFilter.minOrderQty.Value.ToString(CultureInfo.InvariantCulture);
                                                    if (minOrderQty.Contains('.'))
                                                    {

                                                        placeOrder.qty = Math.Round(((((coinWalletBalance.equity - totalAmount.Sum()) * 3) / 100).Value) / lastPrice,
                                                       getInstrumentsInfo[placeOrder.symbol].lotSizeFilter.minOrderQty.Value.ToString(CultureInfo.InvariantCulture).Split('.')[1].Length
                                                       , MidpointRounding.ToEven);
                                                    }
                                                    else
                                                    {

                                                        placeOrder.qty = Math.Round(((((coinWalletBalance.equity - totalAmount.Sum()) * 3) / 100).Value) / lastPrice,
                                                       0
                                                       , MidpointRounding.ToEven);
                                                    }

                                               
                                                }
                                                else
                                                {
                                                    placeOrder.qty = Math.Round(((((coinWalletBalance.equity - totalAmount.Sum()) * 3) / 100).Value) / lastPrice, 0, MidpointRounding.ToEven);
                                                }
                                                string qty = placeOrder.qty.Value.ToString(CultureInfo.InvariantCulture);

                                                double stopLoss = (((lastPrice * 30) / 100));

                                                if (placeOrder.side.Equals(EnumConverter.GetString(OrderSide.Buy)))
                                                {
                                                    stopLoss = lastPrice - stopLoss;

                                                }
                                                if (placeOrder.side.Equals(EnumConverter.GetString(OrderSide.Sell)))
                                                {
                                                    stopLoss = lastPrice + stopLoss;
                                                }
                                           
                                                if (tickersCategorySymbol.result.list.FirstOrDefault().lastPrice.Value.ToString(CultureInfo.InvariantCulture).Contains("."))
                                                {

                                                   placeOrder.stopLoss = Math.Round(stopLoss, tickersCategorySymbol.result.list.FirstOrDefault().lastPrice.Value.ToString(CultureInfo.InvariantCulture).Split('.')[1].Length, MidpointRounding.ToEven);
                                                }
                                                else
                                                {//tickersCategorySymbol.result.list.FirstOrDefault().lastPrice.Value
                                                    placeOrder.stopLoss = Math.Round(stopLoss, 0, MidpointRounding.ToEven);
                                                }
                                                // placeOrder.stopLoss = 
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


                                            //placeOrder.stopLoss = "10";
                                            RootOrderReponse createPositionByBit = await _byBitApiService.PlaceOrder(placeOrder);
                                            bool setLeverage = false;

                                            if (createPositionByBit.result.orderId != null)
                                            {
                                                totalAmount.Add(variationtotalAmount);
                                                variationtotalAmount = (((coinWalletBalance.equity - totalAmount.Sum()) * 30) / 100).Value;
                                            }
                                            else
                                            {
                                                //createPositionByBit.result.
                                                File.AppendAllLines("WriteLinesLogPositionNotAdd.txt", new[] { string.Concat("amount :", variationtotalAmount, "*******position not add*********", DateTime.Now.ToString("yyyy/MM/dd HH:mm:ss:fff")) });
                                                File.AppendAllLines("WriteLinesLogPositionNotAdd.txt", new[] { string.Concat("retCode  => ", createPositionByBit.retCode,
                                                " retMsg  => ", createPositionByBit.retMsg," symbol  => ", placeOrder.symbol) });
                                                // File.AppendAllLines("WriteLinesLogPositionNotAdd.txt", new[] { string.Concat(" retMsg  => ", createPositionByBit.retMsg) });
                                                // File.AppendAllLines("WriteLinesLogPositionNotAdd.txt", new[] { string.Concat(" symbol  => ", placeOrder.symbol) });

                                            }

                                            if (itemUser.IdTelegrame == 1476318624 && createPositionByBit.result.orderId != null)
                                            {
                                                /* switchCrossIsolatedMargin = await _byBitApiService.SwitchCrossIsolatedMargin(new SwitchIsolated()
                                                {
                                                    ApiKey = itemUser.ApiKey,
                                                    SecretKey = itemUser.SecretKey,
                                                    Symbol = itemPosition.symbol,
                                                    Category = EnumConverter.GetString(Category.Linear),//"linear",
                                                    BuyLeverage = itemPosition.leverage.ToString(),
                                                    SellLeverage = itemPosition.leverage.ToString(),
                                                    TradeMode = 1
                                                });*/
                                                int intSetLeverage;
                                                bool success = int.TryParse(itemPosition.leverage.ToString(), out intSetLeverage);

                                                //intSetLeverage = 5;
                                                if (intSetLeverage > 30)
                                                {
                                                    intSetLeverage = 30;
                                                }

                                               /* if (intSetLeverage < 10)
                                                {
                                                    intSetLeverage = 20;
                                                }*/

                                                /*if (intSetLeverage > getInstrumentsInfo[placeOrder.symbol].leverageFilter.maxLeverage)
                                                {
                                                    intSetLeverage = getInstrumentsInfo[placeOrder.symbol].leverageFilter.maxLeverage;
                                                }*/

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

                                                    /*switchCrossIsolatedMargin = await _byBitApiService.SwitchCrossIsolatedMargin(new SwitchIsolated()
                                                    {
                                                        ApiKey = itemUser.ApiKey,
                                                        SecretKey = itemUser.SecretKey,
                                                        Symbol = itemPosition.symbol,
                                                        Category = EnumConverter.GetString(Category.Linear),//"linear",
                                                        BuyLeverage = "30",
                                                        SellLeverage = "30",
                                                        TradeMode = 1
                                                    });*/
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
                                                        item.LongShort, item.nickName);
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
                                                        Amount = totalAmount.LastOrDefault()
                                                    });
                                                }


                                            }
                                        }

                                    }
                                }
                            }
                        }

                        //delete doublon dans Binance_ByBit_Users et add orderForDeleted
                      //orderForDeleted.AddRange( await _binanceOrderService.GetBinanceByBitOrdersForByEncryptedUid(item.code));
                    }
                }
            }
         //   orderForDeleted = orderForDeleted.Distinct().ToList();
          /*  foreach (var itemUser in allUserActive)
            {
                foreach ( var itemOrder in orderForDeleted)
                {
                    foreach (var itemOritemBinanceByBitOrders in itemOrder.BinanceByBitOrders)
                    {
                        await _byBitApiService.CancelOrder(itemOrder, itemOritemBinanceByBitOrders.ByBitOrderId,itemUser.ApiKey, itemUser.SecretKey);
                    }
                }
               
            }*/

           /* foreach (var item in positionsData)
            {
                await _binanceOrderService.DeleteBinanceByBitOrdersForByEncryptedUid(item.code);
            }
            await _binanceOrderService.DeletedOrders(orderForDeleted);*/
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
