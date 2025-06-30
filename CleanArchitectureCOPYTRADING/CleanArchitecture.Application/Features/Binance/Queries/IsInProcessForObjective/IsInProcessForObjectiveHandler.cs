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

namespace CleanArchitecture.Application.Features.Binance.Queries.IsInProcessForObjective
{
    public class IsInProcessForObjectiveHandler : IRequestHandler<IsInProcessForObjectiveQuery, bool>
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
        public IConfiguration _configuration { get; }


        public IsInProcessForObjectiveHandler(IOptions<JwtSettings> jwtSettings, IOptions<BinanceBybitSettings> binanceBybitSettings, IUnitOfWork unitOfWork,  ITelegrameBotService telegrameBotService,
            IDonetZipService donetZipService, IFileService fileService, IByBitApiService byBitService,
            IBinanceTraderService binanceTraderService, IBinanceOrderService binanceOrderService,
            IBinanceByBitUsersService binanceByBitUsersService,
            IBinanceByBitOrderService binanceByBitOrderService,
            IBinanceMonitoringProcessService binanceMonitoringProcessService,
            IBinanceTraderPerformanceService loadTraderPerformanceService,
            IConfiguration configuration, IBinanceMonitoringCoinWalletBalanceObjectiveProcessService monitoringCoinWalletBalanceObjectiveProcessService)
        {
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

        public async Task<bool> Handle(IsInProcessForObjectiveQuery request, CancellationToken cancellationToken)
        {

            if (request.IdTelegrame==0)
            {
                throw new ArgumentException("IdTelegrame mandatory");
            }
            var userActive = (await _binanceByBitUsersService.GetAllIsactive()).Where(p=>p.IdTelegrame== request.IdTelegrame).FirstOrDefault() ;

            var walletBalance = await _byBitApiService.WalletBalance(userActive.ApiKey, userActive.SecretKey);
            while (walletBalance == null)
            {
                Thread.Sleep(5000);
                walletBalance = await _byBitApiService.WalletBalance(userActive.ApiKey, userActive.SecretKey);

                // code block to be executed
            }
            var coinWalletBalance = await _byBitApiService.GetCoinFromWalletBalance(walletBalance);
            bool IsIngProcessForObjective = false;
            IsIngProcessForObjective = (await _monitoringCoinWalletBalanceObjectiveProcessService.IsIngProcessForObjective(request.IdTelegrame, coinWalletBalance));
            return IsIngProcessForObjective;
        }
        /*
         
            public GetTraderUrlForUpdatePositionBinanceHandler(IBinanceTraderPerformanceService loadTraderPerformanceService,
             PredictionEnginePool<TraderDataPerformanceBinance, ModelTraderDataOutput> traderPredictionEngine,IBinanceTraderService binanceTraderService,
             IConfiguration configuration,
             IBinanceTraderUrlForUpdatePositionBinanceQueryService binanceTraderUrlForUpdatePositionBinanceQueryService,
             IBinanceMonitoringCoinWalletBalanceObjectiveProcessService monitoringCoinWalletBalanceObjectiveProcessService)
        {
            _loadTraderPerformanceService = loadTraderPerformanceService ?? throw new ArgumentException(nameof(loadTraderPerformanceService));
            _traderPredictionEngine = traderPredictionEngine ?? throw new ArgumentException(nameof(traderPredictionEngine));
            _binanceTraderService = binanceTraderService ?? throw new ArgumentException(nameof(binanceTraderService));
            _configuration = configuration ?? throw new ArgumentException(nameof(configuration));
            _binanceTraderUrlForUpdatePositionBinanceQueryService = binanceTraderUrlForUpdatePositionBinanceQueryService ?? throw new ArgumentException(nameof(binanceTraderUrlForUpdatePositionBinanceQueryService));
            _monitoringCoinWalletBalanceObjectiveProcessService = monitoringCoinWalletBalanceObjectiveProcessService ?? throw new ArgumentException(nameof(monitoringCoinWalletBalanceObjectiveProcessService));

        }

        public async Task<List<string>> Handle(GetTraderUrlForUpdatePositionBinanceQuery request, CancellationToken cancellationToken)
        {

            

            //List<string> result = await _loadTraderPerformanceService.TraderUrlForUpdatePositionBinance();
            //new version for test
            var result = await _binanceTraderUrlForUpdatePositionBinanceQueryService.GetTraderUrlForUpdatePositionBinance();
            return result.Select(p=>p.EncryptedUid).Take(int.Parse(_configuration.GetSection("BinanceBybitSettings:TotalTraderUrlForUpdatePositionBinanceQuery").Value)).ToList();
            
        }
    }
         */
    }
}
