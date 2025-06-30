using CleanArchitecture.Application.Contracts.Infrastructure;
using Microsoft.Extensions.Logging;
using CleanArchitecture.Application.Contracts.Persistence;
using CleanArchitecture.Domain.Binance;

namespace CleanArchitecture.Infrastructure.Services
{

    public class BinanceMonitoringProcessService : IBinanceMonitoringProcessService
    {
        private readonly IBinanceMonitoringProcessRepository _binanceMonitoringProcessRepository;
        public ILogger<BinanceMonitoringProcessService> _logger { get; }

        public BinanceMonitoringProcessService() { }

        public BinanceMonitoringProcessService(ILogger<BinanceMonitoringProcessService> logger,
            IBinanceMonitoringProcessRepository binanceMonitoringProcessRepository)
        {
            _logger = logger;
            _binanceMonitoringProcessRepository = binanceMonitoringProcessRepository ?? throw new ArgumentException(nameof(binanceMonitoringProcessRepository));
        }

        public async Task<BinanceMonitoringProcess> Create(BinanceMonitoringProcess binanceMonitoringProcess)
        {
            return await _binanceMonitoringProcessRepository.Create(binanceMonitoringProcess);
        }
        public async Task<bool> Update(BinanceMonitoringProcess binanceMonitoringProcess)
        {
            return await _binanceMonitoringProcessRepository.Update(binanceMonitoringProcess);
        }
        public async Task<bool> GetIsIngProcess()
        {
            return await _binanceMonitoringProcessRepository.GetIsIngProcess();
        }
        public async Task<BinanceMonitoringProcess> GetLatBinanceMonitoringProcess()
        {
            return await _binanceMonitoringProcessRepository.GetLatBinanceMonitoringProcess();
        }
    }
}
