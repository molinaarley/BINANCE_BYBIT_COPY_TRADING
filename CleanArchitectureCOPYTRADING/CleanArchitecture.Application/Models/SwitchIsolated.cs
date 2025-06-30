
namespace CleanArchitecture.Application.Models
{
    public class SwitchIsolated
    {
        public string Category { get; set; }
        public string Symbol { get; set; }
        public int TradeMode { get; set; }
        public string BuyLeverage { get; set; }
        public string SellLeverage { get; set; }
        public string ApiKey { get; set; }
        public string SecretKey { get; set; }
    }
}
