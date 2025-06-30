
namespace CleanArchitecture.Application.Models
{
    public class GetOpenOrders
    {
        public string apiKey { get; set; }
        public string secretKey { get; set; }
        public string category { get; set; } = "inverse";
        public string? symbol { get; set; } = null;
        public string? baseCoin { get; set; } = null;
        public string? settleCoin { get; set; } = null;
        public string? orderId { get; set; } = null;
        public string? orderLinkId { get; set; } = null;
        public int? openOnly { get; set; } = null;
        public string? orderFilter { get; set; } = null;
        public int? limit { get; set; } = null;
        public string? cursor { get; set; } = null;
        public string? baseAsset { get; set; } = null;
        public string? settleAsset { get; set; } = null;
    }
}
