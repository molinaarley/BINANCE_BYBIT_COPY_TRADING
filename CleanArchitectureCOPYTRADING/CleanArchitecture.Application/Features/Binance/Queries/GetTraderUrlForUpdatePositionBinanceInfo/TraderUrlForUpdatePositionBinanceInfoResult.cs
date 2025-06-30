using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CleanArchitecture.Application.Features.Binance.Queries.GetTraderUrlForUpdatePositionBinanceInfo
{
    public class TraderUrlForUpdatePositionBinanceInfoResult
    {
        public string EncryptedUid { get; set; } = null!;
        public string NickName { get; set; } = null!;
        public DateTime? CreatedOn { get; set; }
        public int? RankTrader { get; set; }
        public bool? PositionShared { get; set; }
        public int? FollowerCount { get; set; }
    }
}
