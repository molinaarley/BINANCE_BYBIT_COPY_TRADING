using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CleanArchitecture.Application.Models
{
    public class DataInfoUser
    {
        public string nickName { get; set; }
        public string userPhotoUrl { get; set; }
        public bool positionShared { get; set; }
        public bool deliveryPositionShared { get; set; }
        public int followingCount { get; set; }
        public int followerCount { get; set; }
        public string twitterUrl { get; set; }
        public string introduction { get; set; }
        public bool twShared { get; set; }
        public bool? isTwTrader { get; set; }
        public object openId { get; set; }
        public object portfolioId { get; set; }
    }

    public class RootInfoUser
    {
        public string code { get; set; }
        public object message { get; set; }
        public object messageDetail { get; set; }
        public DataInfoUser data { get; set; }
        public bool success { get; set; }
    }

}
