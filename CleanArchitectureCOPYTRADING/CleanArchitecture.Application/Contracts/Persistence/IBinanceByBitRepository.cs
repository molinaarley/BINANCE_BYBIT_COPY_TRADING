using CleanArchitecture.Application.Models;
using CleanArchitecture.Domain;
using CleanArchitecture.Domain.Binance;

namespace CleanArchitecture.Application.Contracts.Persistence
{
    public interface IBinanceTraderRepository 
    {
        Task<string> Create(BinanceTrader binanceTrader);
        Task<List<BinanceTrader>> GetAll();
        Task<Dictionary<string, string>> GetAllDictionary();
        Task<BinanceTrader> Update(BinanceTrader binanceTrader);
        Task<BinanceTraderFromJsonReponseRoot> LoadBinanceTraderFromJson(string filePath);
        Task<bool> UpdateRange(List<BinanceTrader> binanceTrader);
        Task<BinanceTrader> GetByUid(string uid);

    }
}
