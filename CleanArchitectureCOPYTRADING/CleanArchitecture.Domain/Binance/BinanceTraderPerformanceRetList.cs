using System;
using System.Collections.Generic;
using CleanArchitecture.Domain.Common;

namespace CleanArchitecture.Domain.Binance
{
    public partial class BinanceTraderPerformanceRetList : BaseDomainModel
    {
        public int Id { get; set; }
        public string EncryptedUid { get; set; } = null!;
        public string? PeriodType { get; set; }
        public string? StatisticsType { get; set; }
        public double? Value { get; set; }
        public int? Rank { get; set; }
        public string? GuidNewItem { get; set; }
        public virtual BinanceTrader EncryptedU { get; set; } = null!;
    }
}
