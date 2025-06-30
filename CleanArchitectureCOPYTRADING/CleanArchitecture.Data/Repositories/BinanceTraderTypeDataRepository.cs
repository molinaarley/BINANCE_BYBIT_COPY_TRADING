using CleanArchitecture.Application.Contracts.Persistence;
using CleanArchitecture.Domain.Binance;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace CleanArchitecture.Infrastructure.Repositories
{
    public class BinanceTraderTypeDataRepository : IBinanceTraderTypeDataRepository
    {
       private readonly IBinanceContext _binanceContext;
        public ILogger<BinanceTraderTypeDataRepository> _logger { get; }
        public BinanceTraderTypeDataRepository(ILogger<BinanceTraderTypeDataRepository> logger, IBinanceContext binanceContext) 
        {
            _logger = logger;
            _binanceContext = binanceContext ?? throw new ArgumentException(nameof(binanceContext));
        }

        public async Task<string> Create(BinanceTraderTypeData binanceTraderTypeData)
        {
             try
             {
                await  _binanceContext.BinanceTraderTypeData.AddAsync(binanceTraderTypeData);
                await _binanceContext.SaveChangesAsync();
                return binanceTraderTypeData.EncryptedUid;
             }
             catch (Exception ex)
             {
                _logger.LogError(ex, $"Error creating library {binanceTraderTypeData.EncryptedUid}");
                throw ex;
             }
        }

        public async Task<BinanceTraderTypeData> Update(BinanceTraderTypeData binanceTraderTypeData)
        {
            try
            {
                var updateEntity = _binanceContext.BinanceTraderTypeData.Update(binanceTraderTypeData);
                await _binanceContext.SaveChangesAsync();
                return updateEntity.Entity;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error creating library {binanceTraderTypeData.EncryptedUid}");
                throw ex;
            }
        }
        public async Task<List<BinanceTraderTypeData>> GetAll()
        {
            var result =await  _binanceContext.BinanceTraderTypeData.AsNoTracking().ToListAsync();
            return result;
        }

        public async Task<List<BinanceTraderTypeData>> GetAllByTypeData(string typeData)
        {
            var result = await _binanceContext.BinanceTraderTypeData.AsNoTracking().
                Where(p=>p.TypeData.ToLower().Equals(typeData.ToLower())).ToListAsync();
            return result;
        }

        public async Task<Dictionary<string,string>> GetAllDictionary()
        {
            var result = (await GetAll())
           .ToDictionary(p => p.EncryptedUid, p => p.EncryptedUid );
            return result;
        }


        public async Task<bool> DeletedAll()
        {
            try
            {
                var allItem = _binanceContext.BinanceTraderTypeData.ToList();
                _binanceContext.BinanceTraderTypeData.RemoveRange(allItem);
                await _binanceContext.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error DeletedAll BinanceTraderTypeData");
                throw ex;
            }
            return true;
        }


    }
}
