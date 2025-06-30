using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Reflection;
using AutoMapper;
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
using Microsoft.ML;
using Newtonsoft.Json;

namespace CleanArchitecture.Application.Features.BinanceTraderIA.Queries.GetTraderUrlForUpdatePositionBinanceIA
{
    public class GetTraderUrlForUpdatePositionBinanceHandler : IRequestHandler<GetTraderUrlForUpdatePositionBinanceIAQuery, List<string>>
    {
        private readonly IBinanceTraderPerformanceService _loadTraderPerformanceService;
        private readonly IBinanceTraderService _binanceTraderService;
        public IConfiguration _configuration { get; }
        private readonly MLContext _mlContext;
        public GetTraderUrlForUpdatePositionBinanceHandler(IBinanceTraderPerformanceService loadTraderPerformanceService, IBinanceTraderService binanceTraderService, IConfiguration configuration,
             MLContext mlContext)
        {
            _loadTraderPerformanceService = loadTraderPerformanceService ?? throw new ArgumentException(nameof(loadTraderPerformanceService));
            _binanceTraderService = binanceTraderService ?? throw new ArgumentException(nameof(binanceTraderService));
            _configuration = configuration;
            _mlContext = mlContext ?? throw new ArgumentException(nameof(mlContext));
        }

        public async Task<List<string>> Handle(GetTraderUrlForUpdatePositionBinanceIAQuery request, CancellationToken cancellationToken)
        {

            Dictionary<string, BinanceTrader> dicBinanceTrader = (await _binanceTraderService.GetAll()).GroupBy(x => x.EncryptedUid).ToDictionary(p => p.Key, p => p.FirstOrDefault());

            var dataTraderDataPerformanceBinance = await _loadTraderPerformanceService.GetAllTraderDataPerformanceBinanceForModel(dicBinanceTrader);
            var dataTraderDataPerformanceBinanceAudit = await _loadTraderPerformanceService.GetAllTraderDataPerformanceBinanceForModelAudit(dicBinanceTrader);
            dataTraderDataPerformanceBinance = dataTraderDataPerformanceBinance.Union(dataTraderDataPerformanceBinanceAudit).ToList();

           var IsTopTraderScorePerformanceScore= Predict(dataTraderDataPerformanceBinance).OrderByDescending(p=>p.Value).Select(p=>p);

            List<string> listUrl = new List<string>();
             return listUrl;
        }


        public Dictionary<string, float> Predict( List<TraderDataPerformanceBinance> newData)
        {
            Dictionary<string, float> result = new Dictionary<string, float>();
            DataViewSchema modelSchema;

            ITransformer trainedModel = _mlContext.Model.Load(_configuration.GetSection("BinanceBybitSettings:Binance_IsTopTraderScore").Value, out modelSchema);
            var predictionEngine = _mlContext.Model.CreatePredictionEngine<TraderDataPerformanceBinance, TraderPerformancePrediction>(trainedModel);
            foreach (var item in  newData)
            {
                if (!result.ContainsKey(item.EncryptedUid))
                {
                    var prediction = predictionEngine.Predict(item);
                    result.Add(item.EncryptedUid, prediction.PerformanceScore);
                }
            }
            return result;
        }
    }
}
