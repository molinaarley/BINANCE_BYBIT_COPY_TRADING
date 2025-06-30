
namespace CleanArchitecture.Application.Models
{
    public class GetPositionInfo
    {
        public string apiKey { get; set; }
        public string secretKey { get; set; }
        public string category { get; set; } = "inverse";
        public string symbol { get; set; }
        public string baseCoin { get; set; }
        public string orderId { get; set; }
        public string orderLinkId { get; set; }
        public int openOnly { get; set; }
        public string orderFilter { get; set; }
        public int limit { get; set; }
        public string cursor { get; set; }
    }
}
