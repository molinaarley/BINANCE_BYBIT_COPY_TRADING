using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CleanArchitecture.Domain.Binance;

namespace CleanArchitecture.Application.Contracts.Persistence
{
    public interface IBinanceMonitoringCoinWalletBalanceObjectiveProcessRepository
    {
        Task<BinanceMonitoringCoinWalletBalanceObjectiveProcess> Create(BinanceMonitoringCoinWalletBalanceObjectiveProcess monitoringCoinWalletBalanceObjectiveProcess);
        Task<bool> GetIsIngProcessForObjective();
        Task<bool> Update(BinanceMonitoringCoinWalletBalanceObjectiveProcess monitoringCoinWalletBalanceObjectiveProcess);
        Task<BinanceMonitoringCoinWalletBalanceObjectiveProcess> GetLastIsIngProcessForObjective(long IdTelegrame);


    }
}
