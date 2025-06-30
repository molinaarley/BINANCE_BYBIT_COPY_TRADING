using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CleanArchitecture.Application.Models
{
    // Root myDeserializedClass = JsonConvert.DeserializeObject<Root>(myJsonResponse);
    public class CreateInternalTransferResult
    {
        public string transferId { get; set; }
    }

    public class CreateInternalTransferRetExtInfo
    {
    }

    public class CreateInternalTransferRoot
    {
        public int retCode { get; set; }
        public string retMsg { get; set; }
        public CreateInternalTransferResult result { get; set; }
        public CreateInternalTransferRetExtInfo retExtInfo { get; set; }
        public long time { get; set; }
    }



}
