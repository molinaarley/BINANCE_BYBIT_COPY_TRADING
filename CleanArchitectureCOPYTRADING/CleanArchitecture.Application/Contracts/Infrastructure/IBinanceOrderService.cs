using CleanArchitecture.Application.Models;
using CleanArchitecture.Domain.Binance;

namespace CleanArchitecture.Application.Contracts.Infrastructure
{
    public interface IBinanceOrderService
    {
        Task<int> Create(BinanceOrder binanceTrader);
        Task<BinanceOrder> GetOrder(BinanceOrder binanceOrder, string EncryptedUid);
        Task<BinanceOrder> getOrderIfNullIsValidationForCreateOrder(BinanceOrder binanceTrader, string EncryptedUid);
        Task<BinanceOrder> GetOrderByUserIdTelegrame(BinanceOrder binanceOrder, long IdTelegrame, string EncryptedUid);
        Task<List<BinanceOrder>> GetBinanceByBitOrdersForByEncryptedUid(string EncryptedUid);
        Task<bool> DeletedOrder(BinanceOrder binanceOrder);
        Task<bool> DeletedOrders(List<BinanceOrder> binanceOrders);
        Task<List<BinanceOrder>> GetOpenOrdersByEncryptedUid(string EncryptedUid);
        Task<List<BinanceOrder>> DeleteBinanceByBitOrdersForByEncryptedUid(string EncryptedUid);
        Task<List<BinanceOrder>> GetAll();
        Task<List<OtherPositionRetList>> GetOtherPositionRetList(List<PositionData> positionsData);

        Task<PlaceOrder> GetPlaceOrderForDeleted(GetPositionInfoResult positionBybitInfo,
          string apiKey, string secretKey, double qty);
        Task<List<BinanceOrderAudit>> GetByDateMax(DateTime date);

    }
}
