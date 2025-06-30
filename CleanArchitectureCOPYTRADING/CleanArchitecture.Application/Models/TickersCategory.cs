using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CleanArchitecture.Application.Models
{
    /* internal class TickersCategory
     {
     }*/
    // Root myDeserializedClass = JsonConvert.DeserializeObject<Root>(myJsonResponse);
    public class TickersCategory
    {
        public string symbol { get; set; }
        public double? lastPrice { get; set; }
        public double? indexPrice { get; set; }
        public double? markPrice { get; set; }
        public double? prevPrice24h { get; set; }
        public double? price24hPcnt { get; set; }
        public double? highPrice24h { get; set; }
        public double? lowPrice24h { get; set; }
        public double? prevPrice1h { get; set; }
        public string openInterest { get; set; }
        public string openInterestValue { get; set; }
        public string turnover24h { get; set; }
        public string volume24h { get; set; }
        public string fundingRate { get; set; }
        public string nextFundingTime { get; set; }
        public string predictedDeliveryPrice { get; set; }
        public string basisRate { get; set; }
        public string deliveryFeeRate { get; set; }
        public string deliveryTime { get; set; }
        public string ask1Size { get; set; }
        public double? bid1Price { get; set; }
        public double? ask1Price { get; set; }
        public string bid1Size { get; set; }
        public string basis { get; set; }
    }

    public class ResultTickersCategory
    {
        public string category { get; set; }
        public List<TickersCategory> list { get; set; }
    }

    public class RetExtInfoTickersCategory
    {
    }

    public class RootTickersCategorySymbol
    {
        public int retCode { get; set; }
        public string retMsg { get; set; }
        public ResultTickersCategory result { get; set; }
        public RetExtInfoTickersCategory retExtInfo { get; set; }
        public long time { get; set; }
    }

}
