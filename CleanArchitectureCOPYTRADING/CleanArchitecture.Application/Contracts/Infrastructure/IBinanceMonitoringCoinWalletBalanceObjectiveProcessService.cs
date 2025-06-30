using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CleanArchitecture.Application.Models;
using CleanArchitecture.Domain.Binance;

namespace CleanArchitecture.Application.Contracts.Infrastructure
{
    public interface IBinanceMonitoringCoinWalletBalanceObjectiveProcessService
    {
        Task<BinanceMonitoringCoinWalletBalanceObjectiveProcess> Create(BinanceMonitoringCoinWalletBalanceObjectiveProcess monitoringCoinWalletBalanceObjectiveProcess);

        Task<bool> Update(BinanceMonitoringCoinWalletBalanceObjectiveProcess monitoringCoinWalletBalanceObjectiveProcess);

        Task<bool> GetIsIngProcessForObjective();
        Task<BinanceMonitoringCoinWalletBalanceObjectiveProcess> GetLastIsIngProcessForObjective(long IdTelegrame);
        Task<bool> IsInObjective(long IdTelegrame,Coin coin);
        Task<bool> IsIngProcessForObjective(long IdTelegrame, Coin coin);
        Task<DateTime> GetInitDateLoadPosition(long IdTelegrame);
    }
}
