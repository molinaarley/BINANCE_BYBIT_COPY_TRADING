using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CleanArchitecture.Domain.Binance;

namespace CleanArchitecture.Application.Contracts.Persistence
{
    public interface IBinanceMonitoringCoinWalletBalanceRepository
    {
        Task<BinanceMonitoringCoinWalletBalance> Create(BinanceMonitoringCoinWalletBalance monitoringCoinWalletBalance);
        Task<List<BinanceMonitoringCoinWalletBalance>> GetALLBinanceMonitoringCoinWalletBalance();

    }
}
