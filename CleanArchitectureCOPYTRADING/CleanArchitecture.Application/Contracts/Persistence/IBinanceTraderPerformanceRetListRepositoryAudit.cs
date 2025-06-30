using CleanArchitecture.Application.Models;
using CleanArchitecture.Domain;
using CleanArchitecture.Domain.Binance;

namespace CleanArchitecture.Application.Contracts.Persistence
{
    public interface IBinanceTraderPerformanceRetListRepositoryAudit
    {
        Task<List<BinanceTraderPerformanceRetListAudit>> GetAll();
      
    }
}
