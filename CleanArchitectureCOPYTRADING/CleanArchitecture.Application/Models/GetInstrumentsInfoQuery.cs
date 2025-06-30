
namespace CleanArchitecture.Application.Models
{
    public class GetInstrumentsInfoQuery
    {
        public string apiKey { get; set; }
        public string secretKey { get; set; }
        public string category { get; set; } = "inverse";
        public string symbol { get; set; }
        public string baseCoin { get; set; }
        public string status { get; set; }
        public int limit { get; set; }
        public string cursor { get; set; }

    }
}
