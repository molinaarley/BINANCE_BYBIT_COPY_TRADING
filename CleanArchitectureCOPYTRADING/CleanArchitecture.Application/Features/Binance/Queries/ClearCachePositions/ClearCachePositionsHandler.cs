using CleanArchitecture.Application.Contracts.Infrastructure;
using MediatR;
using Microsoft.Extensions.Configuration;

namespace CleanArchitecture.Application.Features.Binance.Queries.ClearCachePositions
{
    public class ClearCachePositionsHandler : IRequestHandler<ClearCachePositionsQuery, bool>
    {
      
        private readonly IBinanceCacheByBitSymbolService _binanceCacheByBitSymbolService;
        public IConfiguration _configuration { get; }
        public ClearCachePositionsHandler(IBinanceCacheByBitSymbolService binanceCacheByBitSymbolService)
        {
            _binanceCacheByBitSymbolService = binanceCacheByBitSymbolService ?? throw new ArgumentException(nameof(binanceCacheByBitSymbolService));
        }
        public async Task<bool> Handle(ClearCachePositionsQuery request, CancellationToken cancellationToken)
        {
            await _binanceCacheByBitSymbolService.ClearAllCacheAsync();
            return true;
        }
    }
}
