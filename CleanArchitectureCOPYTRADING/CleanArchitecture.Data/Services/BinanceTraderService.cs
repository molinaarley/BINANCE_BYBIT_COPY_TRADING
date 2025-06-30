using CleanArchitecture.Application.Contracts.Infrastructure;
using CleanArchitecture.Application.Models;
using Microsoft.Extensions.Logging;
using CleanArchitecture.Application.Contracts.Persistence;
using CleanArchitecture.Domain.Binance;

namespace CleanArchitecture.Infrastructure.Services
{

    public class BinanceTraderService : IBinanceTraderService
    {
        private readonly IBinanceTraderRepository _binanceTraderRepository;
        public ILogger<BinanceTraderService> _logger { get; }

        public BinanceTraderService() { }

        public BinanceTraderService(ILogger<BinanceTraderService> logger, IBinanceTraderRepository binanceTraderRepository)
        {
            _logger = logger;
            _binanceTraderRepository = binanceTraderRepository ?? throw new ArgumentException(nameof(binanceTraderRepository));
        }

        public async Task<string> Create(BinanceTrader binanceTrader)
        {
            return await _binanceTraderRepository.Create(binanceTrader);
        }

        public async Task<BinanceTrader> GetByUid(string uid)
        {
            return await _binanceTraderRepository.GetByUid(uid);
        }
         
            public async Task<Dictionary<string, string>> GetAllDictionary()
        {
            var result = await _binanceTraderRepository.GetAllDictionary();
            return result;
        }

        public async Task<List<BinanceTrader>> GetAll()
        {
            var result = await _binanceTraderRepository.GetAll();
            return result;
        }
        public async Task<BinanceTrader> Update(BinanceTrader binanceTrader)
        {
            var result = await _binanceTraderRepository.Update(binanceTrader);
            return result;
        }
        public async Task<bool> UpdateRange(List<BinanceTrader> binanceTrader)
        {
            var result = await _binanceTraderRepository.UpdateRange(binanceTrader);
            return result;

        }
        public async Task<BinanceTraderFromJsonReponseRoot> LoadBinanceTraderFromJson(string filePath)
        {
            var result = await _binanceTraderRepository.LoadBinanceTraderFromJson(filePath);
            return result;
        }
    }
}
