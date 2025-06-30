using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CleanArchitecture.Application.Models
{
    // RootBinancePerformanceRetList myDeserializedClass = JsonConvert.DeserializeObject<RootBinancePerformanceRetList>(myJsonResponse);
    public class DataBinancePerformanceRetList
    {
        public List<PerformanceRetList> performanceRetList { get; set; }
        public long? lastTradeTime { get; set; }
    }

    public class PerformanceRetList
    {
        public string periodType { get; set; }
        public string statisticsType { get; set; }
        public double value { get; set; }
        public int rank { get; set; }
    }

    public class RootBinancePerformanceRetList
    {
        public bool positionsOpen { get; set; }
        public string code { get; set; }
        public object message { get; set; }
        public object messageDetail { get; set; }
        public DataBinancePerformanceRetList data { get; set; }
        public bool success { get; set; }
    }

}
