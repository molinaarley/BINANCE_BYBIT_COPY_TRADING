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
using Newtonsoft.Json;

namespace CleanArchitecture.Application.Features.BinanceTraderIA.Queries.GetAllTraderDataPerformanceBinanceForModel
{
    public class GetAllTraderDataPerformanceBinanceForModelHandler : IRequestHandler<GetAllTraderDataPerformanceBinanceForModelQuery, List<TraderDataPerformanceBinance>>
    {
        private readonly IBinanceTraderPerformanceService _loadTraderPerformanceService;
        private readonly IBinanceTraderService _binanceTraderService;

        public GetAllTraderDataPerformanceBinanceForModelHandler(IBinanceTraderPerformanceService loadTraderPerformanceService,IBinanceTraderService binanceTraderService)
        {
            _loadTraderPerformanceService = loadTraderPerformanceService ?? throw new ArgumentException(nameof(loadTraderPerformanceService));
            _binanceTraderService = binanceTraderService ?? throw new ArgumentException(nameof(binanceTraderService));
        }

        public async Task <List<TraderDataPerformanceBinance>> Handle(GetAllTraderDataPerformanceBinanceForModelQuery request, CancellationToken cancellationToken)
        {
            Dictionary<string, BinanceTrader> dicBinanceTrader = (await _binanceTraderService.GetAll()).GroupBy(x => x.EncryptedUid).ToDictionary(p => p.Key, p => p.FirstOrDefault());
            List<TraderDataPerformanceBinance> result = await _loadTraderPerformanceService.GetAllTraderDataPerformanceBinanceForModel(dicBinanceTrader);

            result.AddRange(await _loadTraderPerformanceService.GetAllTraderDataPerformanceBinanceForModelAudit(dicBinanceTrader));
            return result.ToList();
        }
    }
}
