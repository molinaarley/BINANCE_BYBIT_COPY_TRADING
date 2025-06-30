using CleanArchitecture.Application.Contracts.Persistence;
using CleanArchitecture.Application.Converters;
using CleanArchitecture.Application.Models;
using CleanArchitecture.Domain.Binance;
using CleanArchitecture.Domain.Enum;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace CleanArchitecture.Infrastructure.Repositories
{
 
    public class BinanceMonitoringCoinWalletBalanceObjectiveProcessRepository : IBinanceMonitoringCoinWalletBalanceObjectiveProcessRepository
    {
        private readonly IBinanceContext _binanceContext;
        public ILogger<BinanceMonitoringCoinWalletBalanceObjectiveProcessRepository> _logger { get; }
        public BinanceMonitoringCoinWalletBalanceObjectiveProcessRepository(ILogger<BinanceMonitoringCoinWalletBalanceObjectiveProcessRepository> logger, IBinanceContext binanceContext)
        {
            _logger = logger;
            _binanceContext = binanceContext ?? throw new ArgumentException(nameof(binanceContext));

        }
        public async Task<BinanceMonitoringCoinWalletBalanceObjectiveProcess> Create(BinanceMonitoringCoinWalletBalanceObjectiveProcess monitoringCoinWalletBalanceObjectiveProcess)
        {
            try
            {
                await _binanceContext.BinanceMonitoringCoinWalletBalanceObjectiveProcesses.AddAsync(monitoringCoinWalletBalanceObjectiveProcess);
                await _binanceContext.SaveChangesAsync();
                return monitoringCoinWalletBalanceObjectiveProcess;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error creating BinanceOrderRepository {monitoringCoinWalletBalanceObjectiveProcess.CreatedOn}");
                // throw ex;
                return null;
            }
        }

        public async Task<bool> GetIsIngProcessForObjective()
        {
            var result = _binanceContext.BinanceMonitoringCoinWalletBalanceObjectiveProcesses.OrderByDescending(p => p.CreatedOn).AsNoTracking().AsQueryable();
            var resultData = await result.FirstOrDefaultAsync();
            if (resultData == null || resultData.EndDate.HasValue)
            {
                return false;
            }
            return true;
        }

        public async Task<BinanceMonitoringCoinWalletBalanceObjectiveProcess> GetLastIsIngProcessForObjective(long IdTelegrame)
        {
            var result = _binanceContext.BinanceMonitoringCoinWalletBalanceObjectiveProcesses.Where(p=>p.IdTelegrame== IdTelegrame && !p.EndDate.HasValue).OrderByDescending(p => p.CreatedOn).AsNoTracking().AsQueryable();
            var resultData = await result.FirstOrDefaultAsync();
            if (resultData == null )
            {
                return null;
            }
            return resultData;
        }

        public async Task<bool> Update(BinanceMonitoringCoinWalletBalanceObjectiveProcess monitoringCoinWalletBalanceObjectiveProcess)
        {
            try
            {
                _binanceContext.BinanceMonitoringCoinWalletBalanceObjectiveProcesses.Update(monitoringCoinWalletBalanceObjectiveProcess);
                await _binanceContext.SaveChangesAsync();
                return true;
            }
            catch (Exception e)
            {
                // throw new Exception(
                //     "Create  BinanceMonitoringProcess.", e);
                return false;
            }
        }
    }
}
