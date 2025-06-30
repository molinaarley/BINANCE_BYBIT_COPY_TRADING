using System;
using System.Collections.Generic;

namespace CleanArchitecture.Infrastructure.Persistence.SCAFFOLD_DBCONTEXT.2023-09-17
{
    public partial class BinanceTrader
    {
        public BinanceTrader()
        {
            BinanceOrders = new HashSet<BinanceOrder>();
            BinanceTraderTypeData = new HashSet<BinanceTraderTypeDatum>();
        }

        public string EncryptedUid { get; set; } = null!;
        public string NickName { get; set; } = null!;
        public DateTime? CreatedOn { get; set; }
        public double? Pnl { get; set; }
        public double? Roi { get; set; }
        public int? RankTrader { get; set; }
        public bool? PositionShared { get; set; }
        public int? FollowerCount { get; set; }
        public string? UpdateTime { get; set; }

        public virtual ICollection<BinanceOrder> BinanceOrders { get; set; }
        public virtual ICollection<BinanceTraderTypeDatum> BinanceTraderTypeData { get; set; }
    }
}
