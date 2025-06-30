using CleanArchitecture.Application.Contracts.Persistence;
using CleanArchitecture.Application.Converters;
using CleanArchitecture.Application.Models;
using CleanArchitecture.Domain.Binance;
using CleanArchitecture.Domain.Enum;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace CleanArchitecture.Infrastructure.Repositories
{
 
    public class BinanceTraderUrlForUpdatePositionBinanceQueryRepository : IBinanceTraderUrlForUpdatePositionBinanceQueryRepository
    {
        private readonly IBinanceContext _binanceContext;
        public ILogger<BinanceTraderUrlForUpdatePositionBinanceQueryRepository> _logger { get; }
        public BinanceTraderUrlForUpdatePositionBinanceQueryRepository(ILogger<BinanceTraderUrlForUpdatePositionBinanceQueryRepository> logger, IBinanceContext binanceContext)
        {
            _logger = logger;
            _binanceContext = binanceContext ?? throw new ArgumentException(nameof(binanceContext));

        }
        public async Task<BinanceTraderUrlForUpdatePositionBinanceQuery> Create(BinanceTraderUrlForUpdatePositionBinanceQuery binanceTraderUrlForUpdatePositionBinanceQuery)
        {
            try
            {
                await _binanceContext.BinanceTraderUrlForUpdatePositionBinanceQueries.AddAsync(binanceTraderUrlForUpdatePositionBinanceQuery);
                await _binanceContext.SaveChangesAsync();
                return binanceTraderUrlForUpdatePositionBinanceQuery;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error creating BinanceOrderRepository {binanceTraderUrlForUpdatePositionBinanceQuery.EncryptedUid}");
                return null;
            }
        }


        public async Task<List<BinanceTraderUrlForUpdatePositionBinanceQuery>> GetALLbinanceTraderUrlForUpdatePositionBinanceQuery()
        {
            var result = _binanceContext.BinanceTraderUrlForUpdatePositionBinanceQueries.AsNoTracking().AsQueryable();


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
            return new List<BinanceTraderUrlForUpdatePositionBinanceQuery>();
        }


        public async Task<List<BinanceTraderUrlForUpdatePositionBinanceQuery>> GetTraderUrlForUpdatePositionBinance()
        {
            var resultList = _binanceContext.BinanceTraderUrlForUpdatePositionBinanceQueries.OrderByDescending(p=>p.BinanceMonitoringCoinWalletBalanceObjectiveProcessId).Take(1).AsNoTracking().AsQueryable();
            BinanceTraderUrlForUpdatePositionBinanceQuery resultDataObj = resultList.FirstOrDefault();
           var result=  _binanceContext.BinanceTraderUrlForUpdatePositionBinanceQueries.Where(p => p.BinanceMonitoringCoinWalletBalanceObjectiveProcessId == resultDataObj.BinanceMonitoringCoinWalletBalanceObjectiveProcessId).AsNoTracking().AsQueryable();
           return result.ToList();
        }
    }
}
