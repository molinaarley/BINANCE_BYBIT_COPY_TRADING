using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CleanArchitecture.Application.Models;
using CleanArchitecture.Domain.Binance;

namespace CleanArchitecture.Application.Contracts.Infrastructure
{
    public interface IBinanceTraderUrlForUpdatePositionBinanceQueryService
    {
        Task<BinanceTraderUrlForUpdatePositionBinanceQuery> Create(BinanceTraderUrlForUpdatePositionBinanceQuery monitoringCoinWalletBalanceObjectiveProcess);

        Task<List<BinanceTraderUrlForUpdatePositionBinanceQuery>> GetALLbinanceTraderUrlForUpdatePositionBinanceQuery();
        Task<bool> AddEncryptedUidList(List<string> encryptedUid, int BinanceMonitoringCoinWalletBalanceObjectiveProcessId);
        Task<List<BinanceTraderUrlForUpdatePositionBinanceQuery>> GetTraderUrlForUpdatePositionBinance();
    }
}
