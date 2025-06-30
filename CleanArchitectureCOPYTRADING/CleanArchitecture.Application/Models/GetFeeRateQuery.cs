using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CleanArchitecture.Application.Models
{
    public class GetFeeRateQuery
    {
        public string apiKey { get; set; }
        public string secretKey { get; set; }
        public string category { get; set; } = "inverse";
        public string symbol { get; set; }
        public string baseCoin { get; set; }
    }
}
