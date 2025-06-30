using System;
using System.Collections.Generic;

namespace CleanArchitecture.Infrastructure.Persistence.SCAFFOLD_DBCONTEXT.2024-06-12
{
    public partial class BinanceTraderPerformanceRetListAudit
    {
        public int Id { get; set; }
        public string EncryptedUid { get; set; } = null!;
        public DateTime? CreatedOn { get; set; }
        public string? PeriodType { get; set; }
        public string? StatisticsType { get; set; }
        public double? Value { get; set; }
        public int? Rank { get; set; }
        public string? GuidNewItem { get; set; }

        public virtual BinanceTrader EncryptedU { get; set; } = null!;
    }
}
