using System;
using System.Collections.Generic;

namespace CleanArchitecture.Infrastructure.Persistence.SCAFFOLD_DBCONTEXT.2024-06-11
{
    public partial class BinanceOrderAudit
    {
        public int Id { get; set; }
        public string Symbol { get; set; } = null!;
        public double? Amount { get; set; }
        public double? EntryPrice { get; set; }
        public double? Leverage { get; set; }
        public double? MarkPrice { get; set; }
        public double? Pnl { get; set; }
        public bool? TradeBefore { get; set; }
        public DateTime? UpdateTime { get; set; }
        public DateTime? CreatedOn { get; set; }
        public double? UpdateTimeStamp { get; set; }
        public bool? Yellow { get; set; }
        public string EncryptedUid { get; set; } = null!;
        public string Side { get; set; } = null!;
        public bool IsForClosed { get; set; }
        public DateTime? DeleteOn { get; set; }
    }
}
