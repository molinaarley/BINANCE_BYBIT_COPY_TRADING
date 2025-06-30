
namespace CleanArchitecture.Application.Models
{
    public class GetPositionInfoV3Query
    {
        public string apiKey { get; set; }
        public string secretKey { get; set; }
        public string symbol { get; set; }
        public string settleCoin { get; set; }
    }
}
