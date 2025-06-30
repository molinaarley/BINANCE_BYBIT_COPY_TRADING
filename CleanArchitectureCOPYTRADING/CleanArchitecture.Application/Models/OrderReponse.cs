using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CleanArchitecture.Application.Models
{
    public class ResultOrderReponse
    {
        public string orderId { get; set; }
        public string orderLinkId { get; set; }
    }

    public class RetExtInfoOrderReponse
    {
    }

    public class RootOrderReponse
    {
        public int retCode { get; set; }
        public string retMsg { get; set; }
        public ResultOrderReponse result { get; set; }
        public RetExtInfoOrderReponse retExtInfo { get; set; }
        public long time { get; set; }
    }


}
