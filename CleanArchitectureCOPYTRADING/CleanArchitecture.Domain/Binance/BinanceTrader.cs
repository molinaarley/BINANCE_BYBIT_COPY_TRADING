using System;
using System.Collections.Generic;
using CleanArchitecture.Domain.Common;

namespace CleanArchitecture.Domain.Binance
{
    public partial class BinanceTrader: BaseDomainModel
    {
        public BinanceTrader()
        {
            BinanceOrders = new HashSet<BinanceOrder>();
            BinanceTraderTypeData = new HashSet<BinanceTraderTypeData>();
            BinanceTraderPerformanceRetList = new HashSet<BinanceTraderPerformanceRetList>();
        }

        public string EncryptedUid { get; set; } = null!;
        public string NickName { get; set; } = null!;
        public DateTime? CreatedOn { get; set; }
        public int? RankTrader { get; set; }
        public bool? PositionShared { get; set; }
        public int? FollowerCount { get; set; }
        public string? UpdateTime { get; set; }
        public virtual ICollection<BinanceOrder> BinanceOrders { get; set; }
        public virtual ICollection<BinanceTraderTypeData> BinanceTraderTypeData { get; set; }

        public virtual ICollection<BinanceTraderPerformanceRetList> BinanceTraderPerformanceRetList { get; set; }
        public virtual ICollection<BinanceTraderPerformanceRetListAudit> BinanceTraderPerformanceRetListAudit { get; set; }
    }
}
