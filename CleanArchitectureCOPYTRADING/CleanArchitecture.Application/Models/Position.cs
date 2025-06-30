using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AutoMapper.Execution;

namespace CleanArchitecture.Application.Models
{
    
    public class Position
    {
        public string Symbol { get; set; }
        public float? Amount { get; set; }
        public float? EntryPrice { get; set; }
        public float? Leverage { get; set; }
        public float? MarkPrice { get; set; }
        public float? Pnl { get; set; }
        public bool TradeBefore { get; set; }
        public bool Yellow { get; set; }
        public bool EncryptedUid { get; set; }
  
    }



    public class OtherPositionRetList
    {
        public string symbol { get; set; }
       public double? entryPrice { get; set; }
        public double? markPrice { get; set; }
        public double? pnl { get; set; }
        public double? roe { get; set; }
        public IList<int> updateTime { get; set; }
        public double? amount { get; set; }
        public string updateTimeStamp { get; set; }
       public bool yellow { get; set; }
        public bool tradeBefore { get; set; }
        public double? leverage { get; set; }
        public string? side { get; set; }

    }
    public class Data
    {
        public IList<OtherPositionRetList> otherPositionRetList { get; set; }
        public IList<int> updateTime { get; set; }
        public string updateTimeStamp { get; set; }

    }
    public class PositionData
    {
        public string code { get; set; }
        public string message { get; set; }
        public string messageDetail { get; set; }
        public Data data { get; set; }
        public bool success { get; set; }
        public string nickName { get; set; }
        

    }
}
