using CleanArchitecture.Application.Contracts.Infrastructure;
using Microsoft.Extensions.Logging;
using CleanArchitecture.Application.Contracts.Persistence;
using CleanArchitecture.Domain.Binance;
using CleanArchitecture.Application.Models;
using Microsoft.Extensions.Configuration;

namespace CleanArchitecture.Infrastructure.Services
{
    /// <summary>
    /// 
    /// </summary>
    public class BinanceMonitoringCoinWalletBalanceService : IBinanceMonitoringCoinWalletBalanceService
    {
        private readonly IBinanceMonitoringCoinWalletBalanceRepository _monitoringCoinWalletBalanceRepository;
        public ILogger<BinanceMonitoringCoinWalletBalanceService> _logger { get; }
        public IConfiguration _configuration { get; }
        public BinanceMonitoringCoinWalletBalanceService() { }

        public BinanceMonitoringCoinWalletBalanceService(ILogger<BinanceMonitoringCoinWalletBalanceService> logger,
            IBinanceMonitoringCoinWalletBalanceRepository monitoringCoinWalletBalanceRepository, IConfiguration configuration)
        {
            _logger = logger;
            _monitoringCoinWalletBalanceRepository = monitoringCoinWalletBalanceRepository ?? throw new ArgumentException(nameof(monitoringCoinWalletBalanceRepository));
            _configuration = configuration ?? throw new ArgumentException(nameof(configuration));
        }

        public async Task<BinanceMonitoringCoinWalletBalance> Create(BinanceMonitoringCoinWalletBalance monitoringCoinWalletBalanceObjectiveProcess)
        {
            return await _monitoringCoinWalletBalanceRepository.Create(monitoringCoinWalletBalanceObjectiveProcess);
        }

        public async Task<List<BinanceMonitoringCoinWalletBalance>> GetALLBinanceMonitoringCoinWalletBalance()
        {
            return await _monitoringCoinWalletBalanceRepository.GetALLBinanceMonitoringCoinWalletBalance();
        }

    }
}
