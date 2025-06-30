using CleanArchitecture.Application.Models;
using CleanArchitecture.Domain;
using CleanArchitecture.Domain.Binance;

namespace CleanArchitecture.Application.Contracts.Persistence
{
    public interface IBinanceOrderAuditRepository
    {
        Task<List<BinanceOrderAudit>> GetByDateMax(DateTime date);
        Task<int> Create(BinanceOrderAudit binanceOrderAudit);
    }
}


