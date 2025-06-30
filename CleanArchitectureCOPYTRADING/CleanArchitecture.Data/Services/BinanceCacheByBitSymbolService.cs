using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Caching.Memory;
using CleanArchitecture.Application.Contracts.Infrastructure;
using CleanArchitecture.Application.Models;

namespace CleanArchitecture.Infrastructure.Services
{

    public class BinanceCacheByBitSymbolService : IBinanceCacheByBitSymbolService
    {
        private readonly IMemoryCache _memoryCache;
        public ILogger<BinanceCacheByBitSymbolService> _logger { get; }
        public BinanceCacheByBitSymbolService(ILogger<BinanceCacheByBitSymbolService> logger, IMemoryCache memoryCache)
        {
            _logger = logger;
            _memoryCache = memoryCache ?? throw new ArgumentException(nameof(memoryCache));


            if (System.IO.File.Exists("CacheByBitSymbol.txt"))
            {
                var lines = System.IO.File.ReadAllLines("CacheByBitSymbol.txt");

                foreach (var line in lines)
                {

                    var key = line.Trim();
                    if (!string.IsNullOrEmpty(key))
                    {
                        string cacheResult;
                        if (!_memoryCache.TryGetValue(key, out cacheResult))
                        {
                            _memoryCache.Set(key, key, new MemoryCacheEntryOptions()
                            .SetAbsoluteExpiration(TimeSpan.FromDays(5)));
                        }
                    }
                }
            }
        }
        public async Task<bool> Add(string symbol)
        {
            string cacheResult;
            if (!_memoryCache.TryGetValue(symbol, out cacheResult))
            {
                _memoryCache.Set(symbol, symbol,
                  new MemoryCacheEntryOptions()
                  .SetAbsoluteExpiration(TimeSpan.FromDays(5)));
                File.AppendAllLines("CacheByBitSymbol.txt", new[] { symbol });
                return true;
            }
            return false;
        }

        public async Task<Dictionary<string,string>> GetDictionaryInCache()
        {
            Dictionary<string, string> allSymbol = System.IO.File.ReadAllLines("CacheByBitSymbol.txt").ToList().ToDictionary(symbol => symbol, symbol => symbol);
            return allSymbol;
        }

        public async Task<bool> HasSymbolInCache(string symbol)
        {
            string cacheResult;
            if (_memoryCache.TryGetValue(symbol, out cacheResult))
            {
                return true;
            }
            return false;
        }
        public async Task ClearAllCacheAsync()
        {
             if (_memoryCache is MemoryCache memoryCache)
             {
                 await Task.Run(() => memoryCache.Compact(1.0));
             }            
            File.WriteAllText("CacheByBitSymbol.txt", string.Empty);
        }

        public async Task<int> GetCacheKeyCountAsync()
        {
            if (_memoryCache is MemoryCache memoryCache)
            {
                // Run the count operation asynchronously
                return await Task.Run(() => memoryCache.Count);
            }

            return 0;
        }
    }
}
