using CleanArchitecture.Application.Models;
using CleanArchitecture.Domain;
using CleanArchitecture.Domain.Binance;

namespace CleanArchitecture.Application.Contracts.Persistence
{
    public interface IBinanceMonitoringProcessRepository
    {
        Task<BinanceMonitoringProcess> Create(BinanceMonitoringProcess binanceMonitoringProcess);
        Task<bool> Update(BinanceMonitoringProcess binanceMonitoringProcess);
        Task<bool> GetIsIngProcess();
        Task<BinanceMonitoringProcess> GetLatBinanceMonitoringProcess();


       // Task<Dictionary<long, List<BinanceByBitOrder>>> GetAllInDictionary();
    }
}
