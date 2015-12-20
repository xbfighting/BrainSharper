using System;
using BrainSharper.Abstract.MathUtils.DistanceMeaseures;
using BrainSharper.Abstract.MathUtils.Normalizers;

namespace BrainSharper.Implementations.Algorithms.Knn.BackwardsElimination
{
    public class BackwardsEliminationKnnClassifier<TPredictionResult> : BackwardsEliminationPredictor<TPredictionResult>
    {
        public BackwardsEliminationKnnClassifier(
            IDistanceMeasure distanceMeasure,
            IQuantitativeDataNormalizer dataNormalizer,
            Func<double, double> weightingFunc = null,
            IDistanceMeasure similarityMeasure = null,
            bool normalizeNumericValues = false)
            : base(
                distanceMeasure, dataNormalizer, VoteForBestCategoricalValue, weightingFunc, similarityMeasure,
                normalizeNumericValues)
        {
        }
    }
}