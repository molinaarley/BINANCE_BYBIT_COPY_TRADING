using CleanArchitecture.Application.Contracts.Persistence;
using CleanArchitecture.Application.Converters;
using CleanArchitecture.Application.Models;
using CleanArchitecture.Domain.Binance;
using CleanArchitecture.Domain.Enum;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace CleanArchitecture.Infrastructure.Repositories
{
 
    public class BinanceMonitoringCoinWalletBalanceRepository : IBinanceMonitoringCoinWalletBalanceRepository
    {
        private readonly IBinanceContext _binanceContext;
        public ILogger<BinanceMonitoringCoinWalletBalanceRepository> _logger { get; }
        public BinanceMonitoringCoinWalletBalanceRepository(ILogger<BinanceMonitoringCoinWalletBalanceRepository> logger, IBinanceContext binanceContext)
        {
            _logger = logger;
            _binanceContext = binanceContext ?? throw new ArgumentException(nameof(binanceContext));

        }
        public async Task<BinanceMonitoringCoinWalletBalance> Create(BinanceMonitoringCoinWalletBalance monitoringCoinWalletBalance)
        {
            try
            {
                await _binanceContext.BinanceMonitoringCoinWalletBalances.AddAsync(monitoringCoinWalletBalance);
                await _binanceContext.SaveChangesAsync();
                return monitoringCoinWalletBalance;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error creating BinanceOrderRepository {monitoringCoinWalletBalance.CreatedOn}");
                return null;
            }
        }


        public async Task<List<BinanceMonitoringCoinWalletBalance>> GetALLBinanceMonitoringCoinWalletBalance()
        {
            var result = _binanceContext.BinanceMonitoringCoinWalletBalances.OrderByDescending(p => p.CreatedOn).AsNoTracking().AsQueryable();


            try
            {
                var resultData = await result.ToListAsync();
                if (resultData == null)
                {
                    return null;
                }
                return resultData;
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
                //  Block of code to handle errors
            }
            return new List<BinanceMonitoringCoinWalletBalance>();

            
        }
    }
}
