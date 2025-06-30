using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CleanArchitecture.Domain.Binance
{
    public partial class BinanceMonitoringProcess
    {
        public int Id { get; set; }
        public DateTime? CreatedOn { get; set; }
        public DateTime? EndDate { get; set; }
    }
}
