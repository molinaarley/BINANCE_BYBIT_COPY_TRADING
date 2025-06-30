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

namespace CleanArchitecture.Application.Features.Binance.Queries.GetTraderUrlForUpdatePositionBinanceInfo
{
    public class GetTraderUrlForUpdatePositionBinanceInfoHandler : IRequestHandler<GetTraderUrlForUpdatePositionBinanceInfoQuery, List<string>>
    {
        private readonly IBinanceTraderPerformanceService _loadTraderPerformanceService;
        public GetTraderUrlForUpdatePositionBinanceInfoHandler(IBinanceTraderPerformanceService loadTraderPerformanceService)
        {
            _loadTraderPerformanceService = loadTraderPerformanceService ?? throw new ArgumentException(nameof(loadTraderPerformanceService));
        }

        public async Task<List<string>> Handle(GetTraderUrlForUpdatePositionBinanceInfoQuery request, CancellationToken cancellationToken)
        {
            var result = await _loadTraderPerformanceService.GetTraderForUpdatePositionFromBinance();
            List<string> listUrl = new List<string>();

            foreach (var item in result)
            {
                if (item.Value > 3)
                {
                    listUrl.Add(item.Key);
                }
            }
            return listUrl/*.Where(p=> !p.Equals("FAD84AAFD6E43900BF15E06B21857715") 
            )*/.Distinct().ToList();
            //2154D02AD930F6C6E65C507DD73CB3E7

            //D3AFE978B3F0CD58489BC27B35906769
        }
    }
}
