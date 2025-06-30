using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CleanArchitecture.Application.Models;
using CleanArchitecture.Domain.Binance;

namespace CleanArchitecture.Application.Contracts.Infrastructure
{
    public interface IBinanceMonitoringCoinWalletBalanceService
    {
        Task<BinanceMonitoringCoinWalletBalance> Create(BinanceMonitoringCoinWalletBalance monitoringCoinWalletBalanceObjectiveProcess);

        Task<List<BinanceMonitoringCoinWalletBalance>> GetALLBinanceMonitoringCoinWalletBalance();
    }
}
