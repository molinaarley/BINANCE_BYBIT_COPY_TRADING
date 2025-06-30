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
using Microsoft.ML;
using Newtonsoft.Json;

namespace CleanArchitecture.Application.Features.Binance.Queries.MonitoringCoinWalletBalanceObjective
{
    public class MonitoringCoinWalletBalanceObjectiveHandler : IRequestHandler<MonitoringCoinWalletBalanceObjectiveQuery, bool>
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
        private readonly IBinanceTraderUrlForUpdatePositionBinanceQueryService _binanceTraderUrlForUpdatePositionBinanceQueryService;
        public readonly BinanceBybitSettings _binanceBybitSettings;
        private readonly JwtSettings _jwtSettings;
        public  IConfiguration _configuration { get; }


        public MonitoringCoinWalletBalanceObjectiveHandler(IOptions<JwtSettings> jwtSettings, IOptions<BinanceBybitSettings> binanceBybitSettings, IUnitOfWork unitOfWork,  ITelegrameBotService telegrameBotService,
            IDonetZipService donetZipService, IFileService fileService, IByBitApiService byBitService,
            IBinanceTraderService binanceTraderService, IBinanceOrderService binanceOrderService,
            IBinanceByBitUsersService binanceByBitUsersService,
            IBinanceByBitOrderService binanceByBitOrderService,
            IBinanceMonitoringProcessService binanceMonitoringProcessService,
            IBinanceTraderPerformanceService loadTraderPerformanceService,
            IConfiguration configuration, IBinanceMonitoringCoinWalletBalanceObjectiveProcessService monitoringCoinWalletBalanceObjectiveProcessService,
            IBinanceTraderUrlForUpdatePositionBinanceQueryService binanceTraderUrlForUpdatePositionBinanceQueryService)
        {
            _binanceTraderUrlForUpdatePositionBinanceQueryService = binanceTraderUrlForUpdatePositionBinanceQueryService ?? throw new ArgumentException(nameof(binanceTraderUrlForUpdatePositionBinanceQueryService));
            _monitoringCoinWalletBalanceObjectiveProcessService = monitoringCoinWalletBalanceObjectiveProcessService ?? throw new ArgumentException(nameof(monitoringCoinWalletBalanceObjectiveProcessService));
            _donetZipService = donetZipService ?? throw new ArgumentException(nameof(donetZipService));
            _unitOfWork = unitOfWork;
            _binanceBybitSettings = binanceBybitSettings.Value;
            _jwtSettings = jwtSettings.Value;

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
        }

        public async Task<bool> Handle(MonitoringCoinWalletBalanceObjectiveQuery request, CancellationToken cancellationToken)
        {
            Console.WriteLine(_binanceBybitSettings.BinanceOrderqty);
            Console.WriteLine(_jwtSettings.Key);

            var allUserActive = await _binanceByBitUsersService.GetAllIsactive();
            if (allUserActive == null)
                return false; 


            foreach (var itemUser in allUserActive)
            {
                var lastIsIngProcessForObjective = await _monitoringCoinWalletBalanceObjectiveProcessService.GetLastIsIngProcessForObjective(itemUser.IdTelegrame);

                //NOUS OBTENONS LE MONTANT D'ARGENT QUE VOUS AVEZ SUR VOTRE COMPTE BYBIT
                var walletBalance = await _byBitApiService.WalletBalance(itemUser.ApiKey, itemUser.SecretKey);
                while (walletBalance == null)
                {
                    Thread.Sleep(5000);
                    walletBalance = await _byBitApiService.WalletBalance(itemUser.ApiKey, itemUser.SecretKey);

                    // code block to be executed
                }
                var coinWalletBalance = await _byBitApiService.GetCoinFromWalletBalance(walletBalance);

                if (lastIsIngProcessForObjective == null || lastIsIngProcessForObjective.EndDate.HasValue)
                {
                
                var result=   await _monitoringCoinWalletBalanceObjectiveProcessService.Create(new BinanceMonitoringCoinWalletBalanceObjectiveProcess()
                   {
                       CreatedOn = DateTime.Now,
                       Equity = coinWalletBalance.equity.Value,
                       EquityObjective = coinWalletBalance.equity.Value + int.Parse(_configuration.GetSection("BinanceBybitSettings:BinanceObjectiveDailY").Value),
                       IdTelegrame = itemUser.IdTelegrame,
                       UnrealisedPnl = coinWalletBalance.unrealisedPnl.Value,
                       WalletBalance = coinWalletBalance.walletBalance.Value,
                       EndDate=DateTime.Now
                   } );
                    await _binanceTraderUrlForUpdatePositionBinanceQueryService.AddEncryptedUidList(await _loadTraderPerformanceService.TraderUrlForUpdatePositionBinance(),result.Id);
                    result.EndDate = (DateTime?)null;
                    await _monitoringCoinWalletBalanceObjectiveProcessService.Update(result);

                 /*   //   Running backtesting for the new function TraderUrlForUpdatePositionBinanceWithCompositeScore, generating the top 3 traders
                    List<string> tesmp = (await _loadTraderPerformanceService.TraderUrlForUpdatePositionBinanceWithCompositeScore());
                    tesmp.Insert(0,DateTime.Now.ToString("yyyy/MM/dd HH:mm:ss:fff"));
                    File.AppendAllLines("TraderUrlForUpdatePositionBinanceWithCompositeScoreBacktesting.txt", tesmp);*/

                }
                else
                {
                    if (lastIsIngProcessForObjective != null && !lastIsIngProcessForObjective.EndDate.HasValue)
                    {
                       

                        var resultProcess = await _monitoringCoinWalletBalanceObjectiveProcessService.Create(new BinanceMonitoringCoinWalletBalanceObjectiveProcess()
                        {
                            CreatedOn = DateTime.Now,
                            Equity = coinWalletBalance.equity.Value,
                            EquityObjective = coinWalletBalance.equity.Value + int.Parse(_configuration.GetSection("BinanceBybitSettings:BinanceObjectiveDailY").Value),
                            IdTelegrame = itemUser.IdTelegrame,
                            UnrealisedPnl = coinWalletBalance.unrealisedPnl.Value,
                            WalletBalance = coinWalletBalance.walletBalance.Value,
                            EndDate=DateTime.Now
                        });
                        await _binanceTraderUrlForUpdatePositionBinanceQueryService.AddEncryptedUidList(await _loadTraderPerformanceService.TraderUrlForUpdatePositionBinance(), resultProcess.Id);
                        resultProcess.EndDate = (DateTime?)null;
                        await _monitoringCoinWalletBalanceObjectiveProcessService.Update(resultProcess);

                        lastIsIngProcessForObjective.EndDate = DateTime.Now;
                        await _monitoringCoinWalletBalanceObjectiveProcessService.Update(lastIsIngProcessForObjective);

                      /*  //   Running backtesting for the new function TraderUrlForUpdatePositionBinanceWithCompositeScore, generating the top 3 traders
                        List<string> tesmp = (await _loadTraderPerformanceService.TraderUrlForUpdatePositionBinanceWithCompositeScore());
                        tesmp.Insert(0, DateTime.Now.ToString("yyyy/MM/dd HH:mm:ss:fff"));
                        File.AppendAllLines("TraderUrlForUpdatePositionBinanceWithCompositeScoreBacktesting.txt", tesmp);*/
                    }


                }
            }
            return true;
        }
    }
}
