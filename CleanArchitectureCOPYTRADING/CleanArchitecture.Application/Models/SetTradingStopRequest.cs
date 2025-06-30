
namespace CleanArchitecture.Application.Models
{
    public class SetTradingStopRequest
    {
        public string apiKey { get; set; }
        public string secretKey { get; set; }
        public string category { get; set; } = "inverse";
        public string symbol { get; set; }
        public double? takeProfit { get; set; }
        public double? stopLoss { get; set; }
        public int positionIdx { get; set; }
        public string slOrderType { get; set; }
        public double? slLimitPrice { get; set; }                
        public string tpLimitPrice { get; set; }
        public string tpslMode { get; set; }
        public int slSize { get; set; } = 0;
        

    }
}
