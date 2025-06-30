using CleanArchitecture.Application.Contracts.Infrastructure;
using Microsoft.Extensions.Logging;
using CleanArchitecture.Application.Contracts.Persistence;
using CleanArchitecture.Domain.Binance;
using CleanArchitecture.Infrastructure.Persistence;
using CleanArchitecture.Application.Models;

namespace CleanArchitecture.Infrastructure.Services
{

    public class BinanceOrderService : IBinanceOrderService
    {
        private readonly IBinanceOrderRepository _binanceOrderRepository;

        private readonly IBinanceOrderAuditRepository _binanceOrderAuditRepository;

        
        public ILogger<BinanceOrderService> _logger { get; }

        public BinanceOrderService() { }

        public BinanceOrderService(ILogger<BinanceOrderService> logger,
            IBinanceOrderRepository binanceOrderRepository, IBinanceOrderAuditRepository binanceOrderAuditRepository)
        {
            _logger = logger;
            _binanceOrderRepository = binanceOrderRepository ?? throw new ArgumentException(nameof(binanceOrderRepository));
            _binanceOrderAuditRepository = binanceOrderAuditRepository ?? throw new ArgumentException(nameof(binanceOrderAuditRepository));
        }

        public async Task<int> Create(BinanceOrder binanceTrader)
        {
            return await _binanceOrderRepository.Create(binanceTrader);
        }

        public async Task<BinanceOrder> GetOrder(BinanceOrder binanceOrder, string EncryptedUid)
        {
            var result = await _binanceOrderRepository.GetOrder(binanceOrder, EncryptedUid);
            return result;

        }

        public async Task<BinanceOrder> getOrderIfNullIsValidationForCreateOrder(BinanceOrder binanceTrader, string EncryptedUid)
        {
            return await GetOrder(binanceTrader, EncryptedUid);

            // var result= order.BinanceByBitOrders.Where(p=>p.IdTelegrame== IdTelegrame);

            //return !result.Any();

            /*if (order == null)
            {
                return true;
            }
            else
            {
                return false;
            }*/
        }
        public async Task<BinanceOrder> GetOrderByUserIdTelegrame(BinanceOrder binanceOrder, long IdTelegrame, string EncryptedUid)
        {
            var result = await _binanceOrderRepository.GetOrderByUserIdTelegrame(binanceOrder, IdTelegrame, EncryptedUid);
            return result;
        }

        
        public async Task<List<BinanceOrderAudit>> GetByDateMax(DateTime date)
        {
            List<BinanceOrderAudit> result = new List<BinanceOrderAudit>();
            result = await _binanceOrderAuditRepository.GetByDateMax(date);
            return result;
        }



        public async Task<List<BinanceOrder>> GetBinanceByBitOrdersForByEncryptedUid(string EncryptedUid)
        {
            return await _binanceOrderRepository.GetBinanceByBitOrdersForByEncryptedUid(EncryptedUid);
        }

        public async Task<List<BinanceOrder>> DeleteBinanceByBitOrdersForByEncryptedUid(string EncryptedUid)
        {
            return await _binanceOrderRepository.DeleteBinanceByBitOrdersForByEncryptedUid(EncryptedUid);
        }

        public async Task<bool> DeletedOrder(BinanceOrder binanceOrder)
        {
            return await _binanceOrderRepository.DeletedOrder(binanceOrder);
        }

        public async Task<bool> DeletedOrders(List<BinanceOrder> binanceOrders)
        {
            return await _binanceOrderRepository.DeletedOrders(binanceOrders);
        }

        public async Task<List<BinanceOrder>> GetOpenOrdersByEncryptedUid(string EncryptedUid)
        {
            return await _binanceOrderRepository.GetOpenOrdersByEncryptedUid(EncryptedUid);
        }

        public async Task<List<BinanceOrder>> GetAll()
        {
            return await _binanceOrderRepository.GetAll();
        }


        public async Task<List<OtherPositionRetList>> GetOtherPositionRetList(List<PositionData> positionsData)
        {
            return await _binanceOrderRepository.GetOtherPositionRetList(positionsData);

        }



        public async Task<PlaceOrder> GetPlaceOrderForDeleted(GetPositionInfoResult positionBybitInfo,
             string apiKey, string secretKey, double qty)
        {
            return await _binanceOrderRepository.GetPlaceOrderForDeleted(positionBybitInfo,
             apiKey, secretKey, qty);
        }


    }
}
