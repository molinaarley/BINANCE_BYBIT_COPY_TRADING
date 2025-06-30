using System;
using System.Collections.Generic;

namespace CleanArchitecture.Infrastructure.Persistence.SCAFFOLD_DBCONTEXT.2023-09-02
{
    public partial class BinanceMonitoringProcess
    {
        public int Id { get; set; }
        public DateTime? CreatedOn { get; set; }
        public DateTime? EndDate { get; set; }
    }
}
