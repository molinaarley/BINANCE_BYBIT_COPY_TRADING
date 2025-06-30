using System;
using System.Collections.Generic;

namespace CleanArchitecture.Infrastructure.Persistence.SCAFFOLD_DBCONTEXT.2024-03-20
{
    public partial class BinanceByBitUser
    {
        public BinanceByBitUser()
        {
            BinanceByBitOrders = new HashSet<BinanceByBitOrder>();
        }

        public long IdTelegrame { get; set; }
        public string? PhoneNumberTelegrame { get; set; }
        public bool? Isactive { get; set; }
        public DateTime Createdate { get; set; }
        public string? LastName { get; set; }
        public string? Name { get; set; }
        public string? ApiKey { get; set; }
        public string? SecretKey { get; set; }

        public virtual ICollection<BinanceByBitOrder> BinanceByBitOrders { get; set; }
    }
}
