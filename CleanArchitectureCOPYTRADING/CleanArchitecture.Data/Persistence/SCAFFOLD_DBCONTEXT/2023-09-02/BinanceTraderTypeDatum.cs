using System;
using System.Collections.Generic;

namespace CleanArchitecture.Infrastructure.Persistence.SCAFFOLD_DBCONTEXT.2023-09-02
{
    public partial class BinanceTraderTypeDatum
    {
        public int Id { get; set; }
        public string EncryptedUid { get; set; } = null!;
        public DateTime? CreatedOn { get; set; }
        public double? Pnl { get; set; }
        public double? Roi { get; set; }
        public string? TypeData { get; set; }

        public virtual BinanceTrader EncryptedU { get; set; } = null!;
    }
}
