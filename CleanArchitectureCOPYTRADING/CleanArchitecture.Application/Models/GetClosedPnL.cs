using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CleanArchitecture.Application.Models
{
    public class GetClosedPnL
    {
        public string apiKey { get; set; }
        public string secretKey { get; set; }
        public string category { get; set; } = "inverse";
        public string symbol { get; set; }
        public int startTime { get; set; }
        public int endTime { get; set; }
        public int limit { get; set; }
        public string cursor { get; set; }
    }
}
