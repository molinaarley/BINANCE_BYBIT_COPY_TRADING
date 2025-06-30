using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CleanArchitecture.Application.Models
{
   
    // Root myDeserializedClass = JsonConvert.DeserializeObject<Root>(myJsonResponse);
    public class GetPositionInfoV3List
    {
        public int positionIdx { get; set; }
        public int riskId { get; set; }
        public string symbol { get; set; }
        public string side { get; set; }
        public string size { get; set; }
        public string positionValue { get; set; }
        public string entryPrice { get; set; }
        public int tradeMode { get; set; }
        public int autoAddMargin { get; set; }
        public string leverage { get; set; }
        public string positionBalance { get; set; }
        public string liqPrice { get; set; }
        public string bustPrice { get; set; }
        public string takeProfit { get; set; }
        public string stopLoss { get; set; }
        public string trailingStop { get; set; }
        public string unrealisedPnl { get; set; }
        public string createdTime { get; set; }
        public string updatedTime { get; set; }
        public string tpSlMode { get; set; }
        public string riskLimitValue { get; set; }
        public string activePrice { get; set; }
        public string markPrice { get; set; }
        public string cumRealisedPnl { get; set; }
        public string positionMM { get; set; }
        public string positionIM { get; set; }
        public string positionStatus { get; set; }
        public string sessionAvgPrice { get; set; }
        public string occClosingFee { get; set; }
        public string avgPrice { get; set; }
        public int adlRankIndicator { get; set; }
    }

    public class GetPositionInfoV3Result
    {
        public List<GetPositionInfoV3List> list { get; set; }
        public string category { get; set; }
        public string nextPageCursor { get; set; }
    }

    public class GetPositionInfoV3RetExtInfo
    {
    }

    public class GetPositionInfoV3Root
    {
        public int retCode { get; set; }
        public string retMsg { get; set; }
        public Result result { get; set; }
        public RetExtInfo retExtInfo { get; set; }
        public long time { get; set; }
    }



}
