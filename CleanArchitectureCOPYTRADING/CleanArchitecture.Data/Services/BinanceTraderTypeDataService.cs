using CleanArchitecture.Application.Contracts.Infrastructure;
using CleanArchitecture.Application.Models;
using Microsoft.Extensions.Logging;
using CleanArchitecture.Application.Contracts.Persistence;
using CleanArchitecture.Domain.Binance;
using CleanArchitecture.Infrastructure.Repositories;

namespace CleanArchitecture.Infrastructure.Services
{

    public class BinanceTraderTypeDataService : IBinanceTraderTypeDataService
    {
        private readonly IBinanceTraderTypeDataRepository _binanceTraderTypeDataRepository;
        public ILogger<BinanceTraderTypeDataService> _logger { get; }

        public BinanceTraderTypeDataService() { }

        public BinanceTraderTypeDataService(ILogger<BinanceTraderTypeDataService> logger, IBinanceTraderTypeDataRepository binanceTraderTypeDataRepository)
        {
            _logger = logger;
            _binanceTraderTypeDataRepository = binanceTraderTypeDataRepository ?? throw new ArgumentException(nameof(binanceTraderTypeDataRepository));
        }

        public async Task<string> Create(BinanceTraderTypeData binanceTrader)
        {
            return await _binanceTraderTypeDataRepository.Create(binanceTrader);
        }
        public async Task<Dictionary<string, string>> GetAllDictionary()
        {
            var result = await _binanceTraderTypeDataRepository.GetAllDictionary();
            return result;
        }


        public async Task<BinanceTraderTypeData> Update(BinanceTraderTypeData binanceTrader)
        {
            var result = await _binanceTraderTypeDataRepository.Update(binanceTrader);
            return result;
        }
        public async Task<List<BinanceTraderTypeData>> GetAllByTypeData(string typeData)
        {
            var result = await _binanceTraderTypeDataRepository.GetAllByTypeData(typeData);
            return result;
        }

        public async Task<bool> DeletedAll()
        {
            var result = await _binanceTraderTypeDataRepository.DeletedAll();
            return result;
        }
    }
}
