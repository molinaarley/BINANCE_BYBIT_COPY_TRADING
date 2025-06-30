using System;
using System.Collections.Generic;

namespace CleanArchitecture.Domain.Binance
{
    public partial class BinanceMonitoringCoinWalletBalance
    {
        public int Id { get; set; }
        public DateTime? CreatedOn { get; set; }
        public double Equity { get; set; }
        public double UnrealisedPnl { get; set; }
        public double WalletBalance { get; set; }
        public long IdTelegrame { get; set; }
        public double? Amount { get; set; }
        public virtual BinanceByBitUser IdTelegrameNavigation { get; set; } = null!;
    }
}
