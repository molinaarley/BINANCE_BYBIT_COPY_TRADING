using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Extensions.ML;
using Microsoft.ML;

namespace CleanArchitecture.Application.Extensions
{
    public static class PredictionEnginePoolExtensions
    {
        public static void UpdateModel<TData, TPrediction>(this PredictionEnginePool<TData, TPrediction> pool, ITransformer model, DataViewSchema modelSchema)
            where TData : class
            where TPrediction : class, new()
        {
            var field = typeof(PredictionEnginePool<TData, TPrediction>).GetField("_predictor", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
            if (field != null)
            {
                field.SetValue(pool, model);
            }

            var schemaField = typeof(PredictionEnginePool<TData, TPrediction>).GetField("_schema", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
            if (schemaField != null)
            {
                schemaField.SetValue(pool, modelSchema);
            }
        }
    }

}
