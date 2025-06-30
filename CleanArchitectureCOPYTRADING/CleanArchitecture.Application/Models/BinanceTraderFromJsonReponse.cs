using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CleanArchitecture.Application.Models
{
    // Root myDeserializedClass = JsonConvert.DeserializeObject<Root>(myJsonResponse);
    public class BinanceTraderFromJsonReponseDatum
    {
        public object futureUid { get; set; }
        public string nickName { get; set; }
        public string userPhotoUrl { get; set; }
        public int rank { get; set; }
        public float? pnl { get; set; }
        public float? roi { get; set; }
        public bool? positionShared { get; set; }
        public object twitterUrl { get; set; }
        public string encryptedUid { get; set; }
        public string updateTime { get; set; }
        public int? followerCount { get; set; }
        public object twShared { get; set; }
        public bool? isTwTrader { get; set; }
        public object openId { get; set; }
    }

    public class BinanceTraderFromJsonReponseRoot
    {
        public string code { get; set; }
        public object message { get; set; }
        public object messageDetail { get; set; }
        public List<BinanceTraderFromJsonReponseDatum> data { get; set; } = new List<BinanceTraderFromJsonReponseDatum>() { };
        public bool success { get; set; }


    }


}
