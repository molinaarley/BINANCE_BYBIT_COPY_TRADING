using System;
using System.Collections.Generic;
using System.Data.SqlTypes;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.ML.Data;

namespace CleanArchitecture.Application.Models
{
    /* public class TraderDataPerformanceBinance
     {
         [LoadColumn(0)]
         public string EncryptedUid { get; set; }

         [LoadColumn(1)]
         public DateTime CreatedOn { get; set; }


         [LoadColumn(2)]
         public float DAILY_ROI { get; set; }

         [LoadColumn(3)]
         public float DAILY_PNL { get; set; }

         [LoadColumn(4)]
         public float EXACT_YEARLY_ROI { get; set; }

         [LoadColumn(5)]
         public float EXACT_YEARLY_PNL { get; set; }
         [LoadColumn(6)]
         public float WEEKLY_ROI { get; set; }

         [LoadColumn(7)]
         public float WEEKLY_PNL { get; set; }

         [LoadColumn(8)]
         public float MONTHLY_ROI { get; set; }

         [LoadColumn(9)]
         public float MONTHLY_PNL { get; set; }

         [LoadColumn(10)]
         public float YEARLY_ROI { get; set; }

         [LoadColumn(11)]
         public float YEARLY_PNL { get; set; }

         [LoadColumn(12)]
         public float ALL_ROI { get; set; }

         [LoadColumn(13)]
         public float ALL_PNL { get; set; }

         [LoadColumn(14)]
         public int RankPeriodType { get; set; }

         [LoadColumn(15)]
         public int RankTrader { get; set; }

         [LoadColumn(16)]
         public float FollowerCount { get; set; }

         [LoadColumn(17)]
         public float IsTopTraderScore { get; set; }
     }*/
    public class TraderDataPerformanceBinance
    {
        [LoadColumn(0)]
        [ColumnName(@"encryptedUid")]
        public string EncryptedUid { get; set; }

        [LoadColumn(1)]
        [ColumnName(@"createdOn")]
        public string CreatedOn { get; set; }

        [LoadColumn(2)]
        [ColumnName(@"dailY_ROI")]
        public float DailY_ROI { get; set; }

        [LoadColumn(3)]
        [ColumnName(@"dailY_PNL")]
        public float DailY_PNL { get; set; }

        [LoadColumn(4)]
        [ColumnName(@"exacT_YEARLY_ROI")]
        public float ExacT_YEARLY_ROI { get; set; }

        [LoadColumn(5)]
        [ColumnName(@"exacT_YEARLY_PNL")]
        public float ExacT_YEARLY_PNL { get; set; }

        [LoadColumn(6)]
        [ColumnName(@"weeklY_ROI")]
        public float WeeklY_ROI { get; set; }

        [LoadColumn(7)]
        [ColumnName(@"weeklY_PNL")]
        public float WeeklY_PNL { get; set; }

        [LoadColumn(8)]
        [ColumnName(@"monthlY_ROI")]
        public float MonthlY_ROI { get; set; }

        [LoadColumn(9)]
        [ColumnName(@"monthlY_PNL")]
        public float MonthlY_PNL { get; set; }

        [LoadColumn(10)]
        [ColumnName(@"yearlY_ROI")]
        public float YearlY_ROI { get; set; }

        [LoadColumn(11)]
        [ColumnName(@"yearlY_PNL")]
        public float YearlY_PNL { get; set; }

        [LoadColumn(12)]
        [ColumnName(@"alL_ROI")]
        public float AlL_ROI { get; set; }

        [LoadColumn(13)]
        [ColumnName(@"alL_PNL")]
        public float AlL_PNL { get; set; }

        [LoadColumn(14)]
        [ColumnName(@"rankPeriodType")]
        public float RankPeriodType { get; set; }

        [LoadColumn(15)]
        [ColumnName(@"rankTrader")]
        public float RankTrader { get; set; }

        [LoadColumn(16)]
        [ColumnName(@"followerCount")]
        public float FollowerCount { get; set; }

        [LoadColumn(17)]
        [ColumnName(@"isTopTraderScore")]
        public float IsTopTraderScore { get; set; }
   
        [ColumnName("IsMonthlY_ROI_Increasing")]
        public float IsMonthlY_ROI_Increasing { get; set; }

        [ColumnName("IsYearlY_ROI_Increasing")]
        public float IsYearlY_ROI_Increasing { get; set; }

        [ColumnName("IsDailY_ROI_Increasing")]
        public float IsDailY_ROI_Increasing { get; set; }
        public float IAIsTopTraderScore { get; set; }
        public float CompositeScore { get; set; } = 0;
    }
}
