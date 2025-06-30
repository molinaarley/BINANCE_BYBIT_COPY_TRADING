using System.Globalization;
using System.Linq;
using System.Reflection;
using AutoMapper;
using CleanArchitecture.Application.Constants;
using CleanArchitecture.Application.Contracts.Infrastructure;
using CleanArchitecture.Application.Contracts.Persistence;
using CleanArchitecture.Application.Converters;
using CleanArchitecture.Application.Features.Binance.Queries.GetTraderUrlForUpdatePositionBinance;
using CleanArchitecture.Application.Models;
using CleanArchitecture.Domain;
using CleanArchitecture.Domain.Binance;
using CleanArchitecture.Domain.Enum;
using MediatR;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.ML;
using Newtonsoft.Json;

namespace CleanArchitecture.Application.Features.Binance.Queries.GetTraderUrlForUpdatePositionBinance
{
    public class GetTraderUrlForUpdatePositionBinanceHandler : IRequestHandler<GetTraderUrlForUpdatePositionBinanceQuery, List<string>>
    {
        private readonly IBinanceTraderPerformanceService _loadTraderPerformanceService;
        private readonly PredictionEnginePool<TraderDataPerformanceBinance, ModelTraderDataOutput> _traderPredictionEngine;
        private readonly IBinanceTraderService _binanceTraderService;
        private readonly IBinanceTraderUrlForUpdatePositionBinanceQueryService _binanceTraderUrlForUpdatePositionBinanceQueryService;
        private readonly IBinanceMonitoringCoinWalletBalanceObjectiveProcessService _monitoringCoinWalletBalanceObjectiveProcessService;
        public IConfiguration _configuration { get; }


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
}
