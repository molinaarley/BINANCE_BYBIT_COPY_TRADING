using System;
using System.Collections.Generic;

namespace CleanArchitecture.Infrastructure.Persistence.SCAFFOLD_DBCONTEXT.2023-09-17
{
    public partial class BinanceByBitOrderAudit
    {
        public int Id { get; set; }
        public string ByBitOrderId { get; set; } = null!;
        public string ByBitOrderLinkId { get; set; } = null!;
        public int BinanceOrderId { get; set; }
        public DateTime? CreatedOn { get; set; }
        public long IdTelegrame { get; set; }
        public DateTime? DeleteOn { get; set; }
        public double? Amount { get; set; }
    }
}
