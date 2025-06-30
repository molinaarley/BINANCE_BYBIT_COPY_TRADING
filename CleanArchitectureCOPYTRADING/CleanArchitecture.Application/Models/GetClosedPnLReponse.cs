using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CleanArchitecture.Application.Models
{
    // Root myDeserializedClass = JsonConvert.DeserializeObject<GetClosedPnLReponseRoot>(myJsonResponse);
    public class GetClosedPnLReponseList
    {
        public string symbol { get; set; }
        public string orderType { get; set; }
        public string leverage { get; set; }
        public string updatedTime { get; set; }
        public string side { get; set; }
        public string orderId { get; set; }
        public string closedPnl { get; set; }
        public string avgEntryPrice { get; set; }
        public double? qty { get; set; }
        public string cumEntryValue { get; set; }
        public string createdTime { get; set; }
        public string orderPrice { get; set; }
        public string closedSize { get; set; }
        public string avgExitPrice { get; set; }
        public string execType { get; set; }
        public string fillCount { get; set; }
        public string cumExitValue { get; set; }
    }

    public class GetClosedPnLReponseResult
    {
        public string nextPageCursor { get; set; }
        public string category { get; set; }
        public List<GetClosedPnLReponseList> list { get; set; }
    }

    public class GetClosedPnLReponseRetExtInfo
    {
    }

    public class GetClosedPnLReponseRoot
    {
        public int retCode { get; set; }
        public string retMsg { get; set; }
        public GetClosedPnLReponseResult result { get; set; }
        public GetClosedPnLReponseRetExtInfo retExtInfo { get; set; }
        public long time { get; set; }
    }


}
