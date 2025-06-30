
namespace CleanArchitecture.Application.Models
{
    public class GetAllCoinsBalance
    {
        public string apiKey { get; set; }
        public string secretKey { get; set; }
        public string accountType { get; set; } 
        public string memberId { get; set; }
        public string coin { get; set; }
        public string withBonus { get; set; }
    }
}
