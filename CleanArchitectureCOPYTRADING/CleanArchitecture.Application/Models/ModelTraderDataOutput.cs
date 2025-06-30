using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AutoMapper.Configuration.Annotations;
using Microsoft.ML.Data;

namespace CleanArchitecture.Application.Models
{
    public class ModelTraderDataOutput
    {
        [Ignore]
        public string EncryptedUidGUID { get; set; }

        [ColumnName(@"encryptedUid")]
        public float[] EncryptedUid { get; set; }

        [ColumnName(@"createdOn")]
        public float[] CreatedOn { get; set; }

        [ColumnName(@"dailY_ROI")]
        public float DailY_ROI { get; set; }

        [ColumnName(@"dailY_PNL")]
        public float DailY_PNL { get; set; }

        [ColumnName(@"exacT_YEARLY_ROI")]
        public float ExacT_YEARLY_ROI { get; set; }

        [ColumnName(@"exacT_YEARLY_PNL")]
        public float ExacT_YEARLY_PNL { get; set; }

        [ColumnName(@"weeklY_ROI")]
        public float WeeklY_ROI { get; set; }

        [ColumnName(@"weeklY_PNL")]
        public float WeeklY_PNL { get; set; }

        [ColumnName(@"monthlY_ROI")]
        public float MonthlY_ROI { get; set; }

        [ColumnName(@"monthlY_PNL")]
        public float MonthlY_PNL { get; set; }

        [ColumnName(@"yearlY_ROI")]
        public float YearlY_ROI { get; set; }

        [ColumnName(@"yearlY_PNL")]
        public float YearlY_PNL { get; set; }

        [ColumnName(@"alL_ROI")]
        public float AlL_ROI { get; set; }

        [ColumnName(@"alL_PNL")]
        public float AlL_PNL { get; set; }

        [ColumnName(@"rankPeriodType")]
        public float RankPeriodType { get; set; }

        [ColumnName(@"rankTrader")]
        public float RankTrader { get; set; }

        [ColumnName(@"followerCount")]
        public float FollowerCount { get; set; }

        [ColumnName(@"Features")]
        public float[] Features { get; set; }

        [ColumnName(@"Score")]
        public float Score { get; set; }


        [ColumnName(@"isTopTraderScore")]
        public float IsTopTraderScore { get; set; }


    }
}
