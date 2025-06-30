using System.Collections.Specialized;
using System.Globalization;
using System.Net.Http;
using System.Text;
using System.Web;
using CleanArchitecture.Application.Contracts.Infrastructure;
using CleanArchitecture.Application.Contracts.Persistence;
using CleanArchitecture.Application.Models;
using CleanArchitecture.Domain;
using CleanArchitecture.Domain.Binance;
using CleanArchitecture.Domain.Enum;
using CleanArchitecture.Infrastructure.Persistence;
using CleanArchitecture.Infrastructure.Services;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using RestSharp;
using System.Linq;
using SendGrid;

namespace CleanArchitecture.Infrastructure.Repositories
{
    public class BinanceTraderRepository :  IBinanceTraderRepository
    {
       private readonly IBinanceContext _binanceContext;
        public ILogger<BinanceTraderRepository> _logger { get; }
        public BinanceTraderRepository(ILogger<BinanceTraderRepository> logger, IBinanceContext binanceContext) 
        {
            _logger = logger;
            _binanceContext = binanceContext ?? throw new ArgumentException(nameof(binanceContext));
        }

        public async Task<string> Create(BinanceTrader binanceTrader)
        {
             try
             {
                await  _binanceContext.BinanceTraders.AddAsync(binanceTrader);
                await _binanceContext.SaveChangesAsync();
                return binanceTrader.EncryptedUid;
             }
             catch (Exception ex)
             {
                _logger.LogError(ex, $"Error creating library {binanceTrader.EncryptedUid}");
                throw ex;
             }
        }

        public async Task<BinanceTrader> Update(BinanceTrader binanceTrader)
        {
            try
            {
                var updateEntity = _binanceContext.BinanceTraders.Update(binanceTrader);
                await _binanceContext.SaveChangesAsync();
                return updateEntity.Entity;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error creating library {binanceTrader.EncryptedUid}");
                throw ex;
            }
        }


        public async Task<bool> UpdateRange(List<BinanceTrader>  binanceTrader)
        {
            try
            {
               _binanceContext.BinanceTraders.UpdateRange(binanceTrader);
                await _binanceContext.SaveChangesAsync();
                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error UpdateRange");
                throw ex;
            }
        }
        public async Task<BinanceTraderFromJsonReponseRoot> LoadBinanceTraderFromJson(string filePath)
        {
            try
            {
                var resultDada = JsonConvert.DeserializeObject<BinanceTraderFromJsonReponseRoot>(File.ReadAllText(filePath));
                return resultDada;
            }
            catch (Exception ex)
            {
           
                throw ex;
            }
            return new BinanceTraderFromJsonReponseRoot();
        }

        public async Task<BinanceTrader> GetByUid(string uid)
        {
            var result = _binanceContext.BinanceTraders.Where(p => p.EncryptedUid.Equals(uid)).
               AsNoTracking().AsQueryable();
            var resultData = await  result.FirstOrDefaultAsync();
            return resultData;
           
        }

        public async Task<List<BinanceTrader>> GetAll()
        {
            var result =await  _binanceContext.BinanceTraders.AsNoTracking().ToListAsync();
            return result;
        }

        public async Task<Dictionary<string,string>> GetAllDictionary()
        {
            var result = (await GetAll())
           .ToDictionary(p => p.EncryptedUid, p => p.EncryptedUid );
            return result;
        }
    }
}
