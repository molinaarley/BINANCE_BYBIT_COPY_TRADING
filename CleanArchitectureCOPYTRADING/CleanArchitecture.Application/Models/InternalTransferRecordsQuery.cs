using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CleanArchitecture.Application.Models
{
    public class CreateInternalTransferQuery
    {
        public string ApiKey { get; set; }
        public string SecretKey { get; set; }
        public string TransferId { get; set; }=Guid.NewGuid().ToString();
        public string Coin { get; set; }
        public string Amount { get; set; }
        public string FromAccountType { get; set; }
        public string ToAccountType { get; set; }      
    }
}
