using System;
using System.Collections.Generic;

namespace CleanArchitecture.WEB
{
    public partial class BinanceTrader
    {
        public BinanceTrader()
        {
            BinanceOrders = new HashSet<BinanceOrder>();
        }

        public string EncryptedUid { get; set; } = null!;
        public string NickName { get; set; } = null!;

        public virtual ICollection<BinanceOrder> BinanceOrders { get; set; }
    }
}
