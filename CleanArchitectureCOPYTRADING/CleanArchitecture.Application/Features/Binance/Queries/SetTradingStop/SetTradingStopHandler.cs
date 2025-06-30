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

namespace CleanArchitecture.Application.Features.Binance.Queries.SetTradingStop
{
    public class SetTradingStopHandler : IRequestHandler<SetTradingStopQuery, bool>
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


        public SetTradingStopHandler(IUnitOfWork unitOfWork,  ITelegrameBotService telegrameBotService,
            IDonetZipService donetZipService, IFileService fileService, IByBitApiService byBitService,
            IBinanceTraderService binanceTraderService, IBinanceOrderService binanceOrderService,
            IBinanceByBitUsersService binanceByBitUsersService,
            IBinanceByBitOrderService binanceByBitOrderService,
            IBinanceMonitoringProcessService binanceMonitoringProcessService,
            IBinanceTraderPerformanceService loadTraderPerformanceService)
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
            _loadTraderPerformanceService = loadTraderPerformanceService ?? throw new ArgumentException(nameof(loadTraderPerformanceService));
        }

        public async Task<bool> Handle(SetTradingStopQuery request, CancellationToken cancellationToken)
        {

            Dictionary<string, BinanceTrader> dicBinanceTrader = (await _binanceTraderService.GetAll()).GroupBy(x => x.EncryptedUid).ToDictionary(p => p.Key, p => p.FirstOrDefault());
            TraderDataPerformanceBinance itemFirstDataPerformance = (await _loadTraderPerformanceService.GetAllTraderDataPerformanceBinanceForModel(dicBinanceTrader)).OrderBy(p=>p.CreatedOn).FirstOrDefault();

            if (itemFirstDataPerformance!=null)
            {
                request.DateBeginForLoad = DateTime.Parse( itemFirstDataPerformance.CreatedOn);
            }
            

            var dicSymboleBD = (await _binanceOrderService.GetAll()).GroupBy(x => x.Symbol).ToDictionary(p => p.Key, p => p.FirstOrDefault().Symbol);//buscar los symbolos en los ultimo 30 minutos
            // File.AppendAllLines("SYMBOLOAS_A_NO_IMPORTAR.txt", new[] { string.Concat(placeOrder.symbol) });
            var readNoIncludeSymbole = File.ReadAllLines("SYMBOLOAS_A_NO_IMPORTAR.txt").ToList().Distinct().ToDictionary(p=>p,p=>p);
           
                      
            //load Position Binance
            List<PositionData> positionsData = new List<PositionData>();
            
            
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
           

                var getInstrumentsInfo = (await _byBitApiService.GetInstrumentsInfo(new GetInstrumentsInfoQuery()
                {
                    apiKey = itemUser.ApiKey,
                    secretKey = itemUser.SecretKey,
                    category = EnumConverter.GetString(Category.Linear)
                })).result.list.GroupBy(x=>x.symbol).ToDictionary(p=>p.Key,p=>p.FirstOrDefault() ) ;

                //var instrumentsInfoInPosition = getInstrumentsInfo.Values.Where(p=> dicSymboleBD.ContainsKey(p.symbol )).ToList();//verificar si tienen stop plos de estos
                var instrumentsInfoInPosition = getInstrumentsInfo.Values.Where(p => p.symbol.Equals("XRPUSDT")).ToList();

                foreach (var item in instrumentsInfoInPosition)
                {

                    var positionBybitInfo = await _byBitApiService.GetPositionInfo(new GetPositionInfo()
                    {
                        category = "linear",
                        symbol = item.symbol,
                        apiKey = itemUser.ApiKey,
                        secretKey = itemUser.SecretKey
                    });

                  
                    double takeProfit = ((positionBybitInfo.list.FirstOrDefault().avgPrice.Value * 70) / 100 )+ positionBybitInfo.list.FirstOrDefault().avgPrice.Value;

                    double stopLoss = (positionBybitInfo.list.FirstOrDefault().avgPrice.Value * 30) / 100;// a finalizar correstamente

                    if ((positionBybitInfo.list.FirstOrDefault().stopLoss.HasValue && positionBybitInfo.list.FirstOrDefault().stopLoss.Value==0 ) || !positionBybitInfo.list.FirstOrDefault().stopLoss.HasValue)
                    {
                        if (positionBybitInfo.list.FirstOrDefault().side.Equals(EnumConverter.GetString(OrderSide.Buy)))
                        {

                           await _byBitApiService.SetTradingStopForPosition(positionBybitInfo.list.FirstOrDefault(), getInstrumentsInfo[item.symbol], itemUser);//GetInstrumentsInfoList








                            stopLoss = positionBybitInfo.list.FirstOrDefault().avgPrice.Value - stopLoss;

                            if (stopLoss.ToString(CultureInfo.InvariantCulture).Contains(".") )
                            {
                                stopLoss = Math.Round(stopLoss,
                                                      positionBybitInfo.list.FirstOrDefault().avgPrice.Value.ToString(CultureInfo.InvariantCulture).Split('.')[1].Length);
                            }


                            await _byBitApiService.SetTradingStop(new SetTradingStopRequest()
                            {
                                apiKey = itemUser.ApiKey,
                                secretKey = itemUser.SecretKey,
                                symbol = item.symbol,
                                positionIdx = 0,
                                //takeProfit = takeProfit,
                                category = EnumConverter.GetString(Category.Linear),
                                stopLoss = stopLoss,
                                slLimitPrice= stopLoss,
                                slOrderType = OrderType.Limit.ToString(),
                                tpslMode= EnumConverter.GetString(TpslMode.Partial)
                            });
                        }
                        else
                        {

                            await _byBitApiService.SetTradingStopForPosition(positionBybitInfo.list.FirstOrDefault(), getInstrumentsInfo[item.symbol], itemUser);//GetInstrumentsInfoList


                            stopLoss = positionBybitInfo.list.FirstOrDefault().avgPrice.Value + stopLoss;

                            if (stopLoss.ToString(CultureInfo.InvariantCulture).Contains("."))
                            {
                                stopLoss = Math.Round(stopLoss,
                                                      positionBybitInfo.list.FirstOrDefault().avgPrice.Value.ToString(CultureInfo.InvariantCulture).Split('.')[1].Length);
                            }

                            await _byBitApiService.SetTradingStop(new SetTradingStopRequest()
                            {
                                apiKey = itemUser.ApiKey,
                                secretKey = itemUser.SecretKey,
                                symbol = item.symbol,
                                positionIdx = 0,
                                //takeProfit = takeProfit,
                                category = EnumConverter.GetString(Category.Linear),
                                stopLoss = stopLoss,
                                slLimitPrice = stopLoss,
                                slOrderType = OrderType.Limit.ToString(),
                                tpslMode = EnumConverter.GetString(TpslMode.Partial)
                            });

                        }
                    }
                }
            }
            //UPDATE IN PROCESS IF NOT THEN CREATE
            // var binanceMonitoringProcess = await _binanceMonitoringProcessService.GetLatBinanceMonitoringProcess();
           
            return true;
        }
    }
}
