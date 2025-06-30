using CleanArchitecture.Application.Contracts.Infrastructure;
using Microsoft.Extensions.Logging;
using CleanArchitecture.Application.Contracts.Persistence;
using CleanArchitecture.Domain.Binance;
using CleanArchitecture.Application.Models;
using Microsoft.Extensions.Configuration;

namespace CleanArchitecture.Infrastructure.Services
{
    /// <summary>
    /// Monitor the wallet and create an algorithm by setting a daily target. If we reach the target, we stop the update for the current day.
    /// </summary>
    public class BinanceMonitoringCoinWalletBalanceObjectiveProcessService : IBinanceMonitoringCoinWalletBalanceObjectiveProcessService
    {
        private readonly IBinanceMonitoringCoinWalletBalanceObjectiveProcessRepository _monitoringCoinWalletBalanceObjectiveProcessRepository;
        public ILogger<BinanceMonitoringCoinWalletBalanceObjectiveProcessService> _logger { get; }
        public IConfiguration _configuration { get; }
        public BinanceMonitoringCoinWalletBalanceObjectiveProcessService() { }

        public BinanceMonitoringCoinWalletBalanceObjectiveProcessService(ILogger<BinanceMonitoringCoinWalletBalanceObjectiveProcessService> logger,
            IBinanceMonitoringCoinWalletBalanceObjectiveProcessRepository monitoringCoinWalletBalanceObjectiveProcessRepository, IConfiguration configuration)
        {
            _logger = logger;
            _monitoringCoinWalletBalanceObjectiveProcessRepository = monitoringCoinWalletBalanceObjectiveProcessRepository ?? throw new ArgumentException(nameof(monitoringCoinWalletBalanceObjectiveProcessRepository));
            _configuration = configuration ?? throw new ArgumentException(nameof(configuration));
        }

        public async Task<BinanceMonitoringCoinWalletBalanceObjectiveProcess> Create(BinanceMonitoringCoinWalletBalanceObjectiveProcess monitoringCoinWalletBalanceObjectiveProcess)
        {
            return await _monitoringCoinWalletBalanceObjectiveProcessRepository.Create(monitoringCoinWalletBalanceObjectiveProcess);
        }

        public async Task<bool> Update(BinanceMonitoringCoinWalletBalanceObjectiveProcess monitoringCoinWalletBalanceObjectiveProcess)
        {
            return await _monitoringCoinWalletBalanceObjectiveProcessRepository.Update(monitoringCoinWalletBalanceObjectiveProcess);
        }
        public async Task<bool> GetIsIngProcessForObjective()
        {
            return await _monitoringCoinWalletBalanceObjectiveProcessRepository.GetIsIngProcessForObjective();
        }

        public async Task<BinanceMonitoringCoinWalletBalanceObjectiveProcess> GetLastIsIngProcessForObjective(long IdTelegrame)
        {
            return await _monitoringCoinWalletBalanceObjectiveProcessRepository.GetLastIsIngProcessForObjective(IdTelegrame);
        }

        public async Task<bool> IsInObjective(long IdTelegrame,Coin coin)
        {
            BinanceMonitoringCoinWalletBalanceObjectiveProcess LastIsIngProcessForObjective = await _monitoringCoinWalletBalanceObjectiveProcessRepository.GetLastIsIngProcessForObjective(IdTelegrame);

            if(LastIsIngProcessForObjective==null ||  LastIsIngProcessForObjective.EndDate.HasValue)
            {
                return true;
            }

            bool isAuthorized = false;
            double totalAuthorizedLoss = (LastIsIngProcessForObjective.Equity * int.Parse(_configuration.GetSection("BinanceBybitSettings:GeneralStopLossAuthorized").Value) ) /100 ;
            //Math.Round(((((coinWalletBalance.equity - totalAmount.Sum()) * binanceOrderqty) / 100).Value) / lastPrice, 2, MidpointRounding.ToEven);

            if (LastIsIngProcessForObjective.Equity> coin.equity)
            {
                isAuthorized = (LastIsIngProcessForObjective.Equity - coin.equity) > totalAuthorizedLoss;
            }


            return( (coin.equity - LastIsIngProcessForObjective.Equity ) > int.Parse(_configuration.GetSection("BinanceBybitSettings:BinanceObjectiveDailY").Value) ) || isAuthorized;
        }


        public async Task<bool> IsIngProcessForObjective(long IdTelegrame,Coin coin)
        {
           
            return  !(await IsInObjective(IdTelegrame, coin)) ;
        }

        public async Task<DateTime> GetInitDateLoadPosition(long IdTelegrame)
        {

            BinanceMonitoringCoinWalletBalanceObjectiveProcess LastIsIngProcessForObjective = await _monitoringCoinWalletBalanceObjectiveProcessRepository.GetLastIsIngProcessForObjective(IdTelegrame);

            if (LastIsIngProcessForObjective!=null && LastIsIngProcessForObjective.CreatedOn.HasValue)
            {
                return LastIsIngProcessForObjective.CreatedOn.Value;
            }
            return DateTime.Now;
        }

    }
}
