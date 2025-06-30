using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CleanArchitecture.Application.Models
{
 // Root myDeserializedClass = JsonConvert.DeserializeObject<Root>(myJsonResponse);
    public class GetBybitServerTimeResult
    {
        public string timeSecond { get; set; }
        public string timeNano { get; set; }
    }

    public class GetBybitServerTimeRetExtInfo
    {
    }

    public class GetBybitServerTimeRoot
    {
        public int retCode { get; set; }
        public string retMsg { get; set; }
        public GetBybitServerTimeResult result { get; set; }
        public GetBybitServerTimeRetExtInfo retExtInfo { get; set; }
        public long time { get; set; }
    }

}
