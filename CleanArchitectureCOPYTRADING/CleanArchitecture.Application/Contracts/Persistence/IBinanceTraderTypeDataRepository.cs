using CleanArchitecture.Application.Models;
using CleanArchitecture.Domain;
using CleanArchitecture.Domain.Binance;

namespace CleanArchitecture.Application.Contracts.Persistence
{
    public interface IBinanceTraderTypeDataRepository
    {
        Task<string> Create(BinanceTraderTypeData binanceTraderTypeData);
        Task<BinanceTraderTypeData> Update(BinanceTraderTypeData binanceTraderTypeData);
        Task<List<BinanceTraderTypeData>> GetAll();
        Task<Dictionary<string, string>> GetAllDictionary();
        Task<List<BinanceTraderTypeData>> GetAllByTypeData(string typeData);
        Task<bool> DeletedAll();
    }
}
