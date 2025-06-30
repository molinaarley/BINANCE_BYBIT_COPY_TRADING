using System;
using System.Collections.Generic;

namespace CleanArchitecture.Domain.Binance
{
    public partial class BinanceByBitUser
    {
        public BinanceByBitUser()
        {
            BinanceByBitOrders = new HashSet<BinanceByBitOrder>();
            BinanceMonitoringCoinWalletBalanceObjectiveProcesses = new HashSet<BinanceMonitoringCoinWalletBalanceObjectiveProcess>();
            BinanceMonitoringCoinWalletBalances = new HashSet<BinanceMonitoringCoinWalletBalance>();
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
        public virtual ICollection<BinanceMonitoringCoinWalletBalanceObjectiveProcess> BinanceMonitoringCoinWalletBalanceObjectiveProcesses { get; set; }
        public virtual ICollection<BinanceMonitoringCoinWalletBalance> BinanceMonitoringCoinWalletBalances { get; set; }


    }
}
