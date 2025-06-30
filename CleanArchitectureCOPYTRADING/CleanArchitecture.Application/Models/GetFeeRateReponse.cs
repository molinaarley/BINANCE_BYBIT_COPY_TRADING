using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CleanArchitecture.Application.Models
{
    // Root myDeserializedClass = JsonConvert.DeserializeObject<GetFeeRateReponseRoot>(myJsonResponse);
    public class GetFeeRateReponseList
    {
        public string symbol { get; set; }
        public string takerFeeRate { get; set; }
        public string makerFeeRate { get; set; }
    }

    public class GetFeeRateReponseResult
    {
        public List<GetFeeRateReponseList> list { get; set; }
    }

    public class GetFeeRateReponseRetExtInfo
    {
    }

    public class GetFeeRateReponseRoot
    {
        public int retCode { get; set; }
        public string retMsg { get; set; }
        public GetFeeRateReponseResult result { get; set; }
        public GetFeeRateReponseRetExtInfo retExtInfo { get; set; }
        public long time { get; set; }
    }


}
