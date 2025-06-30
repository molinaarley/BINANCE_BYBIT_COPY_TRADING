using System;
using System.Collections.Generic;

namespace CleanArchitecture.Domain.Binance
{
    public partial class BinanceMonitoringCoinWalletBalanceObjectiveProcess
    {
        public BinanceMonitoringCoinWalletBalanceObjectiveProcess()
        {
            BinanceTraderUrlForUpdatePositionBinanceQueries = new HashSet<BinanceTraderUrlForUpdatePositionBinanceQuery>();
        }
        public int Id { get; set; }
        public DateTime? CreatedOn { get; set; }
        public double Equity { get; set; }
        public double EquityObjective { get; set; }
        public double UnrealisedPnl { get; set; }
        public double WalletBalance { get; set; }
        public DateTime? EndDate { get; set; }
        public long IdTelegrame { get; set; }

        public virtual BinanceByBitUser IdTelegrameNavigation { get; set; } = null!;
        public virtual ICollection<BinanceTraderUrlForUpdatePositionBinanceQuery> BinanceTraderUrlForUpdatePositionBinanceQueries { get; set; }
    }
}
