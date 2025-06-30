using System;
using System.Collections.Generic;

namespace CleanArchitecture.Domain.Binance
{
    public partial class BinanceTraderUrlForUpdatePositionBinanceQuery
    {
        public int Id { get; set; }
        public string EncryptedUid { get; set; } = null!;
        public int BinanceMonitoringCoinWalletBalanceObjectiveProcessId { get; set; }

        public virtual BinanceMonitoringCoinWalletBalanceObjectiveProcess BinanceMonitoringCoinWalletBalanceObjectiveProcess { get; set; } = null!;
    }
}

