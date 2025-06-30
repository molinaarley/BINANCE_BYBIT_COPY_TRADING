using CleanArchitecture.Domain.Binance;

namespace CleanArchitecture.Application.Contracts.Infrastructure
{
    public interface IBinanceTraderTypeDataService
    {
        Task<string> Create(BinanceTraderTypeData binanceTrader);
        Task<Dictionary<string, string>> GetAllDictionary();
        Task<BinanceTraderTypeData> Update(BinanceTraderTypeData binanceTrader);
        Task<List<BinanceTraderTypeData>> GetAllByTypeData(string typeData);
        Task<bool> DeletedAll();
    }
}
