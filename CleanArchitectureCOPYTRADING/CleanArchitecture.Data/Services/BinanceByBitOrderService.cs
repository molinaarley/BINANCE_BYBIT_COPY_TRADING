using CleanArchitecture.Application.Contracts.Infrastructure;
using Microsoft.Extensions.Logging;
using CleanArchitecture.Application.Contracts.Persistence;
using CleanArchitecture.Domain.Binance;
using CleanArchitecture.Infrastructure.Persistence;
using CleanArchitecture.Infrastructure.Repositories;

namespace CleanArchitecture.Infrastructure.Services
{

    public class BinanceByBitOrderService : IBinanceByBitOrderService
    {
        private readonly IBinanceByBitOrderRepository _binanceByBitOrderRepository;
        public ILogger<BinanceByBitOrderService> _logger { get; }

        public BinanceByBitOrderService() { }

        public BinanceByBitOrderService(ILogger<BinanceByBitOrderService> logger,
            IBinanceByBitOrderRepository binanceByBitOrderRepository)
        {
            _logger = logger;
            _binanceByBitOrderRepository = binanceByBitOrderRepository ?? throw new ArgumentException(nameof(binanceByBitOrderRepository));
        }

        public async Task<BinanceByBitOrder> Create(BinanceByBitOrder binanceByBitOrder)
        {
            return await _binanceByBitOrderRepository.Create(binanceByBitOrder);
        }

        public async Task<BinanceByBitOrder> GetOrder(BinanceByBitOrder binanceByBitOrder)
        {
            var result = await _binanceByBitOrderRepository.GetOrder(binanceByBitOrder);
            return result;
        }

        public async Task<Dictionary<long, List<BinanceByBitOrder>>> GetAllInDictionary()
        {
            var result = await _binanceByBitOrderRepository.GetAllInDictionary();
            return result;
        }


        public async Task<bool> Deleted(BinanceByBitOrder binanceByBitOrder)
        {
            var result = await _binanceByBitOrderRepository.Deleted(binanceByBitOrder);
            return result;

        }


        public async Task<double> GetTotalAmount()
        {

            return await _binanceByBitOrderRepository.GetTotalAmount();

        }

        public async Task<bool> DeletedRange(List<BinanceByBitOrder> binanceByBitOrder)
        {
            var result = await _binanceByBitOrderRepository.DeletedRange(binanceByBitOrder);
            return result;

        }

    }
}
