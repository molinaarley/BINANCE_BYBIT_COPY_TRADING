using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.ML.Data;

namespace CleanArchitecture.Application.Models
{
    public class TraderPerformancePrediction
    {
        [ColumnName("Score")]
        public float PerformanceScore { get; set; }
    }

    public class TraderDataPerformanceBinanceIsDailYROIIncreasingPrediction
    {
        [ColumnName("Score")]
        public float IsDailY_ROI_Increasing { get; set; }

        
    }


    public class TraderDataPerformanceBinanceIsMonthlY_ROI_IncreasingPrediction
    {
        [ColumnName("Score")]
        public float IsMonthlY_ROI_Increasing { get; set; }

    }


    public class TraderDataPerformanceBinanceIsYearlYROIIncreasingPrediction
    {
        [ColumnName("Score")]
        public float IsYearlY_ROI_Increasing { get; set; }
    }
}
