using System;
using System.Collections.Generic;

namespace CleanArchitecture.Infrastructure.Persistence.SCAFFOLD_DBCONTEXT.2024-06-12
{
    public partial class BinanceMonitoringProcess
    {
        public int Id { get; set; }
        public DateTime? CreatedOn { get; set; }
        public DateTime? EndDate { get; set; }
    }
}
