
namespace CleanArchitecture.Application.Models
{
    public class GetTradeHistory
    {
        public string apiKey { get; set; }
        public string secretKey { get; set; }
        public string category { get; set; } = "inverse";
        public string symbol { get; set; }
        public string orderId { get; set; }
        public int startTime { get; set; }
        public int endTime { get; set; }
    }
}
