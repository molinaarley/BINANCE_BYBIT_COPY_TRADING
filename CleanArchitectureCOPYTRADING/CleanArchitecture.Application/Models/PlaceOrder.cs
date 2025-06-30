
using Microsoft.EntityFrameworkCore.Metadata;

namespace CleanArchitecture.Application.Models
{
    public class PlaceOrder
    {
        public string apiKey { get; set; }
        public string secretKey { get; set; }
        public string symbol { get; set; }
        public string category { get; set; }
        public string orderType { get; set; }
        public double? stopLoss { get; set; }
        public string? isLeverage { get; set; }
        public string side { get; set; }
        public double? qty { get; set; }
        public double? price { get; set; }
        public string? timeInForce { get; set; }
        public bool? reduceOnly { get; set; }
        public bool? closeOnTrigger { get; set; }
    }
}
