using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.ML.Data;

namespace CleanArchitecture.Application.Models
{
    public class TraderData
    {
        [LoadColumn(0)] // Etiqueta de clasificación
        public float Label { get; set; }

        [LoadColumn(1)]
        public float EncryptedUid { get; set; }

        [LoadColumn(2)]
        public float RankTrader { get; set; }

        [LoadColumn(3)]
        public float FollowerCount { get; set; }

        [LoadColumn(4)]
        public float RSI_quotidien { get; set; }

        [LoadColumn(5)]
        public float RSI_hebdomadaire { get; set; }

        [LoadColumn(6)]
        public float RSI_mensuel { get; set; }

        [LoadColumn(7)]
        public float G_et_P_quotidien { get; set; }

        [LoadColumn(8)]
        public float G_et_P_hebdomadaire { get; set; }

        [LoadColumn(9)]
        public float G_et_P_mensuel { get; set; }

        [LoadColumn(10)]
        public float G_et_P_total { get; set; }
    }
}
