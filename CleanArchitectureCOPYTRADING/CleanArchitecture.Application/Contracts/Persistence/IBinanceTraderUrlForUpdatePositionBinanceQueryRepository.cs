using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CleanArchitecture.Domain.Binance;

namespace CleanArchitecture.Application.Contracts.Persistence
{
    public interface IBinanceTraderUrlForUpdatePositionBinanceQueryRepository
    {
        Task<BinanceTraderUrlForUpdatePositionBinanceQuery> Create(BinanceTraderUrlForUpdatePositionBinanceQuery monitoringCoinWalletBalance);
        Task<List<BinanceTraderUrlForUpdatePositionBinanceQuery>> GetALLbinanceTraderUrlForUpdatePositionBinanceQuery();
        Task<List<BinanceTraderUrlForUpdatePositionBinanceQuery>> GetTraderUrlForUpdatePositionBinance();

    }
}
