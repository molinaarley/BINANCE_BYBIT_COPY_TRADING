using CleanArchitecture.Application.Contracts.Persistence;
using CleanArchitecture.Application.Converters;
using CleanArchitecture.Application.Models;
using CleanArchitecture.Domain.Binance;
using CleanArchitecture.Domain.Enum;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace CleanArchitecture.Infrastructure.Repositories
{
    public class BinanceOrderAuditRepository : IBinanceOrderAuditRepository
    {
        private readonly IBinanceContext _binanceContext;
        public ILogger<BinanceOrderAuditRepository> _logger { get; }
        public BinanceOrderAuditRepository(ILogger<BinanceOrderAuditRepository> logger, IBinanceContext binanceContext)
        {
            _logger = logger;
            _binanceContext = binanceContext ?? throw new ArgumentException(nameof(binanceContext));

        }
        public async Task<int> Create(BinanceOrderAudit binanceOrderAudit)
        {
            try
            {
                await _binanceContext.BinanceOrderAudits.AddAsync(binanceOrderAudit);
                await _binanceContext.SaveChangesAsync();
                return binanceOrderAudit.Id;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error creating BinanceOrderRepository {binanceOrderAudit.Symbol}");
                // throw ex;
                return 1;
            }
        }

        public async Task<List<BinanceOrderAudit>> GetByDateMax(DateTime date)
        {
            var result = _binanceContext.BinanceOrderAudits.Where(p=> p.DeleteOn.HasValue && p.DeleteOn> date).
               AsNoTracking().AsQueryable();
            return result.ToList();
        }
    }
}
