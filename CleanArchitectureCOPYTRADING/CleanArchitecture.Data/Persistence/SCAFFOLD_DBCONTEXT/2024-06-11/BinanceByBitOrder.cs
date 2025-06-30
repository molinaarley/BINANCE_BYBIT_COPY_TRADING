using System;
using System.Collections.Generic;

namespace CleanArchitecture.Infrastructure.Persistence.SCAFFOLD_DBCONTEXT.2024-06-11
{
    public partial class BinanceByBitOrder
    {
        public int Id { get; set; }
        public string ByBitOrderId { get; set; } = null!;
        public string ByBitOrderLinkId { get; set; } = null!;
        public int BinanceOrderId { get; set; }
        public DateTime? CreatedOn { get; set; }
        public long IdTelegrame { get; set; }
        public double? Amount { get; set; }

        public virtual BinanceOrder BinanceOrder { get; set; } = null!;
        public virtual BinanceByBitUser IdTelegrameNavigation { get; set; } = null!;
    }
}
