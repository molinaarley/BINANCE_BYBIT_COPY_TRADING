using CleanArchitecture.Application.Contracts.Infrastructure;
using MediatR;

namespace CleanArchitecture.Application.Features.Binance.Queries.GetAddIndBinanceTraderPerformanceRetListAudit
{
    public class GetTraderUrlForUpdatePositionBinanceHandler : IRequestHandler<GetAddIndBinanceTraderPerformanceRetListAuditQuery, int>
    {
        private readonly IBinanceTraderPerformanceService _loadTraderPerformanceService;
        public GetTraderUrlForUpdatePositionBinanceHandler(IBinanceTraderPerformanceService loadTraderPerformanceService)
        {
            _loadTraderPerformanceService = loadTraderPerformanceService ?? throw new ArgumentException(nameof(loadTraderPerformanceService));
        }

        public async Task<int> Handle(GetAddIndBinanceTraderPerformanceRetListAuditQuery request, CancellationToken cancellationToken)
        {
            var result = await _loadTraderPerformanceService.AddIndBinanceTraderPerformanceRetListAudit();
            return result;
 ;
        }
    }
}
