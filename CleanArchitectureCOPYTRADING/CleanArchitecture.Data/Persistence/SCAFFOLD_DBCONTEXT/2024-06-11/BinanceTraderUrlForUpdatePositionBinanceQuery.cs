using System;
using System.Collections.Generic;

namespace CleanArchitecture.Infrastructure.Persistence.SCAFFOLD_DBCONTEXT.2024-06-11
{
    public partial class BinanceTraderUrlForUpdatePositionBinanceQuery
    {
        public int Id { get; set; }
        public string EncryptedUid { get; set; } = null!;
        public int BinanceMonitoringCoinWalletBalanceObjectiveProcessId { get; set; }

        public virtual BinanceMonitoringCoinWalletBalanceObjectiveProcess BinanceMonitoringCoinWalletBalanceObjectiveProcess { get; set; } = null!;
    }
}
