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
    public class BinanceTraderUrlForUpdatePositionBinanceQueryService : IBinanceTraderUrlForUpdatePositionBinanceQueryService
    {
        private readonly IBinanceTraderUrlForUpdatePositionBinanceQueryRepository _binanceTraderUrlForUpdatePositionBinanceQueryRepository;
        public ILogger<BinanceTraderUrlForUpdatePositionBinanceQueryService> _logger { get; }
        public IConfiguration _configuration { get; }
        public BinanceTraderUrlForUpdatePositionBinanceQueryService() { }

        public BinanceTraderUrlForUpdatePositionBinanceQueryService(ILogger<BinanceTraderUrlForUpdatePositionBinanceQueryService> logger,
            IBinanceTraderUrlForUpdatePositionBinanceQueryRepository monitoringCoinWalletBalanceRepository, IConfiguration configuration)
        {
            _logger = logger;
            _binanceTraderUrlForUpdatePositionBinanceQueryRepository = monitoringCoinWalletBalanceRepository ?? throw new ArgumentException(nameof(monitoringCoinWalletBalanceRepository));
            _configuration = configuration ?? throw new ArgumentException(nameof(configuration));
        }

        public async Task<BinanceTraderUrlForUpdatePositionBinanceQuery> Create(BinanceTraderUrlForUpdatePositionBinanceQuery monitoringCoinWalletBalanceObjectiveProcess)
        {
            return await _binanceTraderUrlForUpdatePositionBinanceQueryRepository.Create(monitoringCoinWalletBalanceObjectiveProcess);
        }

        public async Task<bool> AddEncryptedUidList(List<string> encryptedUid,int BinanceMonitoringCoinWalletBalanceObjectiveProcessId)
        {

            foreach (var item  in encryptedUid)
            {
               await  Create( new BinanceTraderUrlForUpdatePositionBinanceQuery()
                {
                    BinanceMonitoringCoinWalletBalanceObjectiveProcessId = BinanceMonitoringCoinWalletBalanceObjectiveProcessId,
                    EncryptedUid = item
                });
            }
            return true;
        }

        public async Task<List<BinanceTraderUrlForUpdatePositionBinanceQuery>> GetALLbinanceTraderUrlForUpdatePositionBinanceQuery()
        {
            return await _binanceTraderUrlForUpdatePositionBinanceQueryRepository.GetALLbinanceTraderUrlForUpdatePositionBinanceQuery();
        }
        public async Task<List<BinanceTraderUrlForUpdatePositionBinanceQuery>> GetTraderUrlForUpdatePositionBinance()
        {
            return await _binanceTraderUrlForUpdatePositionBinanceQueryRepository.GetTraderUrlForUpdatePositionBinance();
        }
    }
}
