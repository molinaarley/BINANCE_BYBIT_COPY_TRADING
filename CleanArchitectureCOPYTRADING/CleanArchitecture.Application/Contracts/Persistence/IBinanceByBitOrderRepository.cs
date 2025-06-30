using CleanArchitecture.Application.Models;
using CleanArchitecture.Domain;
using CleanArchitecture.Domain.Binance;

namespace CleanArchitecture.Application.Contracts.Persistence
{
    public interface IBinanceByBitOrderRepository
    {
        Task<BinanceByBitOrder> Create(BinanceByBitOrder binanceByBitOrder);
        Task<List<BinanceByBitOrder>> GetAll();
        Task<BinanceByBitOrder> GetOrder(BinanceByBitOrder binanceByBitOrder);
        Task<Dictionary<long, List<BinanceByBitOrder>>> GetAllInDictionary();
        Task<bool> Deleted(BinanceByBitOrder binanceByBitOrder);
        Task<bool> DeletedRange(List<BinanceByBitOrder> binanceByBitOrder);
         Task<double> GetTotalAmount();
    }
}
