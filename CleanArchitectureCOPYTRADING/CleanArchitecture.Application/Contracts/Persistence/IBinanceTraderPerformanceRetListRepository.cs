using CleanArchitecture.Application.Models;
using CleanArchitecture.Domain;
using CleanArchitecture.Domain.Binance;

namespace CleanArchitecture.Application.Contracts.Persistence
{
    public interface IBinanceTraderPerformanceRetListRepository
    {
        Task<int> AddIndBinanceTraderPerformanceRetListAudit();
        Task<Dictionary<string, int>> GetTraderForUpdatePositionFromBinance();
        Task<bool> DeletedOldPerformanceItem(string guidNewItem);
        Task<bool> Deleted(string encryptedUid);
        Task<string> Create(BinanceTraderPerformanceRetList binanceTraderPerformanceRetList);
        Task<List<BinanceTraderPerformanceRetList>> GetAll();
        Task<Dictionary<string, string>> GetAllDictionary();
        Task<BinanceTraderPerformanceRetList> Update(BinanceTraderPerformanceRetList BinanceTraderPerformanceRetList);
        Task<RootBinancePerformanceRetList> LoadBinanceTraderPerformanceFromJson(string filePath);
        Task<bool> UpdateRange(List<BinanceTraderPerformanceRetList> binanceTraderPerformanceRetList);

        Task<BinanceTraderPerformanceRetList> GetByUidPeriodType(string uid, string periodType);
        Task<bool> DeletedAll();

    }
}
