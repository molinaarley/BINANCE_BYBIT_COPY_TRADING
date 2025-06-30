using System;
using System.Collections.Generic;

namespace CleanArchitecture.Infrastructure.Persistence.SCAFFOLD_DBCONTEXT.2024-06-13
{
    public partial class BinanceTrader
    {
        public BinanceTrader()
        {
            BinanceOrders = new HashSet<BinanceOrder>();
            BinanceTraderPerformanceRetListAudits = new HashSet<BinanceTraderPerformanceRetListAudit>();
            BinanceTraderPerformanceRetLists = new HashSet<BinanceTraderPerformanceRetList>();
            BinanceTraderTypeData = new HashSet<BinanceTraderTypeDatum>();
        }

        public string EncryptedUid { get; set; } = null!;
        public string NickName { get; set; } = null!;
        public DateTime? CreatedOn { get; set; }
        public int? RankTrader { get; set; }
        public bool? PositionShared { get; set; }
        public int? FollowerCount { get; set; }
        public string? UpdateTime { get; set; }
        public int? TopEliableTredor { get; set; }

        public virtual ICollection<BinanceOrder> BinanceOrders { get; set; }
        public virtual ICollection<BinanceTraderPerformanceRetListAudit> BinanceTraderPerformanceRetListAudits { get; set; }
        public virtual ICollection<BinanceTraderPerformanceRetList> BinanceTraderPerformanceRetLists { get; set; }
        public virtual ICollection<BinanceTraderTypeDatum> BinanceTraderTypeData { get; set; }
    }
}
