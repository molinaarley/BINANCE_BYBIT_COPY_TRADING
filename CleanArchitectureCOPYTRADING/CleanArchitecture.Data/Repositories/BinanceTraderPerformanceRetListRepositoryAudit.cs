using CleanArchitecture.Application.Contracts.Persistence;
using CleanArchitecture.Application.Models;
using CleanArchitecture.Domain.Binance;
using CleanArchitecture.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;

namespace CleanArchitecture.Infrastructure.Repositories
{
    public class BinanceTraderPerformanceRetListRepositoryAudit : IBinanceTraderPerformanceRetListRepositoryAudit
    {
       private readonly IBinanceContext _binanceContext;
        public ILogger<BinanceTraderPerformanceRetListRepositoryAudit> _logger { get; }
        public BinanceTraderPerformanceRetListRepositoryAudit(ILogger<BinanceTraderPerformanceRetListRepositoryAudit> logger, IBinanceContext binanceContext) 
        {
            _logger = logger;
            _binanceContext = binanceContext ?? throw new ArgumentException(nameof(binanceContext));
        }

    

        public async Task<List<BinanceTraderPerformanceRetListAudit>> GetAll()
        {
            var result =await  _binanceContext.BinanceTraderPerformanceRetListAudit.AsNoTracking().ToListAsync();
            return result;
        }

        
    }
}
