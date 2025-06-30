
namespace CleanArchitecture.Application.Models
{
    public class GetTickers
    {
        public string category { get; set; } = "inverse";
        public string symbol { get; set; }
        public string baseCoin { get; set; }
        
    }

}
