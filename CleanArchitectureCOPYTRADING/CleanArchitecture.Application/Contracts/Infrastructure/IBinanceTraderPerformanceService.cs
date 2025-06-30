using CleanArchitecture.Application.Contracts.Persistence;
using CleanArchitecture.Application.Models;
using CleanArchitecture.Domain.Binance;

namespace CleanArchitecture.Application.Contracts.Infrastructure
{
    public interface IBinanceTraderPerformanceService
    {
        Task<int> AddIndBinanceTraderPerformanceRetListAudit();
        Task<Dictionary<string, int>> GetTraderForUpdatePositionFromBinance();
        Task<bool> DeletedOldPerformanceItem(string guidNewItem);
        Task<bool> Deleted(string encryptedUid);
        Task<string> Create(BinanceTraderPerformanceRetList binanceTraderPerformanceRetList);
        Task<Dictionary<string, string>> GetAllDictionary();
        Task<BinanceTraderPerformanceRetList> Update(BinanceTraderPerformanceRetList binanceTraderPerformanceRetList);
        Task<bool> UpdateRange(List<BinanceTraderPerformanceRetList> binanceTrader);
        Task<RootBinancePerformanceRetList> LoadBinanceTraderPerformanceFromJson(string filePath);
        Task<bool> DeletedAll();
        Task<Dictionary<string, List<BinanceTraderPerformanceRetList>>> GetAllDictionaryByEncryptedUid();

        Task<List<TraderDataPerformanceBinance>> GetAllTraderDataPerformanceBinanceForModelAudit(Dictionary<string, BinanceTrader> dicBinanceTrader);
        Task<List<TraderDataPerformanceBinance>> GetAllTraderDataPerformanceBinanceForModel(Dictionary<string, BinanceTrader> dicBinanceTrader);
         Task<List<TraderDataPerformanceBinance>> CalculateIncreasingROIs(List<TraderDataPerformanceBinance> data);
        Task<bool> ModelIsMonthlY_ROI_Increasing(List<TraderDataPerformanceBinance> dataTraderDataPerformanceBinance);
        Task<bool> ModelIsYearlY_ROI_Increasing(List<TraderDataPerformanceBinance> dataTraderDataPerformanceBinance);
        Task<bool> ModelIsTopTraderScore(List<TraderDataPerformanceBinance> dataTraderDataPerformanceBinance);
        Task<bool> ModelIsDailY_ROI_Increasing(List<TraderDataPerformanceBinance> dataTraderDataPerformanceBinance);

         Task<bool> ReloadModelIsDailY_ROI_Increasing();
        Task<bool> ReloadModelIsMonthlY_ROI_Increasing();
        Task<bool> ReloadModelIsTopTraderScore();
        Task<List<string>> TraderUrlForUpdatePositionBinance();

        Task<List<string>> TraderUrlForUpdatePositionBinanceWithCompositeScore();
        Task<List<TraderDataPerformanceBinance>> GetAlldataTraderDataPerformanceBinanceAndTraderDataPerformanceBinanceAudit();
    }
}
