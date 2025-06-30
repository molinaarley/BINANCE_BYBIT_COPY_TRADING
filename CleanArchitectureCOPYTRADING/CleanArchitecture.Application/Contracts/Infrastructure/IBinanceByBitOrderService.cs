using CleanArchitecture.Application.Models;
using CleanArchitecture.Domain.Binance;

namespace CleanArchitecture.Application.Contracts.Infrastructure
{
    public interface IBinanceByBitOrderService
    {
        Task<BinanceByBitOrder> Create(BinanceByBitOrder binanceTrader);
        Task<BinanceByBitOrder> GetOrder(BinanceByBitOrder binanceByBitOrder);
        Task<Dictionary<long, List<BinanceByBitOrder>>> GetAllInDictionary();
        Task<bool> Deleted(BinanceByBitOrder binanceByBitOrder);
        Task<bool> DeletedRange(List<BinanceByBitOrder> binanceByBitOrder);
        Task<double> GetTotalAmount();
    }
}
