using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace CleanArchitecture.Application.Models
{
    public class Coin
    {
        public string coin { get; set; }
        public double? equity { get; set; }
        public string usdValue { get; set; }
        public double? walletBalance { get; set; }
        public string borrowAmount { get; set; }
        public string availableToBorrow { get; set; }
        public double? availableToWithdraw { get; set; }
        public string accruedInterest { get; set; }
        public string totalOrderIM { get; set; }
        public double? totalPositionIM { get; set; }
        public string totalPositionMM { get; set; }
        public double? unrealisedPnl { get; set; }
        public double? cumRealisedPnl { get; set; }
    }

    
    public class WalletBalance
    {
        public string accountType { get; set; }
        public string accountIMRate { get; set; }
        public string accountMMRate { get; set; }
        public string totalEquity { get; set; }
        public string totalWalletBalance { get; set; }
        public string totalMarginBalance { get; set; }
        public string totalAvailableBalance { get; set; }
        public string totalPerpUPL { get; set; }
        public string totalInitialMargin { get; set; }
        public string totalMaintenanceMargin { get; set; }
        public string accountLTV { get; set; }
        public List<Coin> coin { get; set; }
    }

    public class Result
    {
        public List<WalletBalance> list { get; set; }
    }

    public class RetExtInfo
    {
    }

    public class Root
    {
        public int retCode { get; set; }
        public string retMsg { get; set; }
        public Result result { get; set; }
        public RetExtInfo retExtInfo { get; set; }
        public long time { get; set; }
    }

}
