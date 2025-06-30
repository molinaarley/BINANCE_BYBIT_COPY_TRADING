using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace CleanArchitecture.Application.Models
{
    // Root myDeserializedClass = JsonConvert.DeserializeObject<Root>(myJsonResponse);
    public class GetPositionInfoResultList
    {
        public int? positionIdx { get; set; }
        public string riskId { get; set; }
        public string riskLimitValue { get; set; }
        public string symbol { get; set; }
        public string side { get; set; }
        public double size { get; set; }
        public double? avgPrice { get; set; }
        public double? positionValue { get; set; }
        public int tradeMode { get; set; }
        public string positionStatus { get; set; }
        public int? autoAddMargin { get; set; }
        public int? adlRankIndicator { get; set; }
        public double? leverage { get; set; }
        public double? positionBalance { get; set; }
        public double? markPrice { get; set; }
        public double? liqPrice { get; set; }
        public double? bustPrice { get; set; }
        public double? positionMM { get; set; }
        public double? positionIM { get; set; }
        public string tpslMode { get; set; }
        public double? takeProfit { get; set; }
        public double? stopLoss { get; set; }
        public double? trailingStop { get; set; }
        public double? unrealisedPnl { get; set; }
        public double? cumRealisedPnl { get; set; }
        public double? curRealisedPnl { get; set; }
        public string createdTime { get; set; }
        public string updatedTime { get; set; }
    }

    public class GetPositionInfoResult
    {
        public List<GetPositionInfoResultList> list { get; set; }
        public string nextPageCursor { get; set; }
        public string category { get; set; }
    }

    public class GetPositionInfoRetExtInfo
    {
    }

    public class GetPositionInfoResultRoot
    {
        public int retCode { get; set; }
        public string retMsg { get; set; }
        public GetPositionInfoResult result { get; set; }
        public GetPositionInfoRetExtInfo retExtInfo { get; set; }
        public long time { get; set; }
    }

}
