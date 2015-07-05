using System;
using BrainSharper.Abstract.MathUtils.DistanceMeaseures;
using BrainSharper.Abstract.MathUtils.Normalizers;

namespace BrainSharper.Implementations.Algorithms.Knn
{
    public class SimpleKnnRegressor : SimpleKnnPredictor<double>
    {
        public SimpleKnnRegressor(
            IDistanceMeasure distanceMeasure, 
            IQuantitativeDataNormalizer dataNormalizer, 
            Func<double, double> weightingFunc = null, 
            IDistanceMeasure similarityMeasure = null, 
            bool normalizeNumericValues = false)
            : base(distanceMeasure, dataNormalizer, FindBestRegressionValue, weightingFunc, similarityMeasure, normalizeNumericValues)
        {
        }
    }
}
