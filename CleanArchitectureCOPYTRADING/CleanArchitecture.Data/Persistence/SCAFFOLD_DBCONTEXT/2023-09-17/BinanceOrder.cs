using System;
using System.Collections.Generic;

namespace CleanArchitecture.Infrastructure.Persistence.SCAFFOLD_DBCONTEXT.2023-09-17
{
    public partial class BinanceOrder
    {
        public BinanceOrder()
        {
            BinanceByBitOrders = new HashSet<BinanceByBitOrder>();
        }

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

        public virtual BinanceTrader EncryptedU { get; set; } = null!;
        public virtual ICollection<BinanceByBitOrder> BinanceByBitOrders { get; set; }
    }
}
