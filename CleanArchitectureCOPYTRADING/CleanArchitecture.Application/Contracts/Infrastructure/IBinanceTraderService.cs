using CleanArchitecture.Application.Contracts.Persistence;
using CleanArchitecture.Application.Models;
using CleanArchitecture.Domain.Binance;

namespace CleanArchitecture.Application.Contracts.Infrastructure
{
    public interface IBinanceTraderService
    {
        Task<List<BinanceTrader>> GetAll();
        Task<string> Create(BinanceTrader binanceTrader);
        Task<Dictionary<string, string>> GetAllDictionary();
        Task<BinanceTrader> Update(BinanceTrader binanceTrader);
        Task<bool> UpdateRange(List<BinanceTrader> binanceTrader);
        Task<BinanceTraderFromJsonReponseRoot> LoadBinanceTraderFromJson(string filePath);
        Task<BinanceTrader> GetByUid(string uid);
    }
}
