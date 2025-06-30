using CleanArchitecture.Application.Contracts.Persistence;
using CleanArchitecture.Application.Models;
using CleanArchitecture.Domain.Binance;

namespace CleanArchitecture.Application.Contracts.Infrastructure
{
    public interface IBinanceCacheByBitSymbolService
    {
        Task<bool> Add(string symbol);
        Task<bool> HasSymbolInCache(string symbol);
        Task ClearAllCacheAsync();
        Task<int> GetCacheKeyCountAsync();
        Task<Dictionary<string, string>> GetDictionaryInCache();
    }
}
