using CleanArchitecture.Application.Models;
using CleanArchitecture.Domain.Binance;

namespace CleanArchitecture.Application.Contracts.Infrastructure
{
    public interface IBinanceMonitoringProcessService
    {
        Task<BinanceMonitoringProcess> Create(BinanceMonitoringProcess binanceMonitoringProcess);
        Task<bool> Update(BinanceMonitoringProcess binanceMonitoringProcess);
        Task<bool> GetIsIngProcess();
        Task<BinanceMonitoringProcess> GetLatBinanceMonitoringProcess();
    }
}
