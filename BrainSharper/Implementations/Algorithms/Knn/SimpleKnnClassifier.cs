using System;
using System.Collections.Generic;
using System.Linq;
using BrainSharper.Abstract.MathUtils.DistanceMeaseures;
using BrainSharper.Abstract.MathUtils.Normalizers;

namespace BrainSharper.Implementations.Algorithms.Knn
{
    public class SimpleKnnClassifier<TPredictionResult> : SimpleKnnPredictor<TPredictionResult>
    {
        public SimpleKnnClassifier(
            IDistanceMeasure distanceMeasure, 
            IQuantitativeDataNormalizer dataNormalizer, 
            Func<double, double> weightingFunc = null, 
            IDistanceMeasure similarityMeasure = null, 
            bool normalizeNumericValues = false) 
            : base(distanceMeasure, dataNormalizer, VoteForBestCategoricalValue, weightingFunc, similarityMeasure, normalizeNumericValues)
        {
        }
    }
}
