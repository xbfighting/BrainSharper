using System.Collections.Generic;
using BrainSharper.Abstract.Algorithms.Knn;
using MathNet.Numerics.LinearAlgebra;

namespace BrainSharper.Implementations.Algorithms.Knn.BackwardsElimination
{
    public class BackwardsEliminationKnnModel<TPredictionResult> : KnnPredictionModel<TPredictionResult>, IBackwardsEliminationKnnModel<TPredictionResult>
    {
        public BackwardsEliminationKnnModel(
            Matrix<double> trainingData,
            IList<TPredictionResult> expectedTrainingOutcomes,
            IList<string> dataColumnsNames, 
            int kNeighbors,
            bool useWeightedDistance,
            IList<IBackwardsEliminationRemovedFeatureData> removedFeaturesData,
            double accuracy = 0)
            : base(trainingData, expectedTrainingOutcomes, dataColumnsNames, kNeighbors, useWeightedDistance, accuracy)
        {
            RemovedFeaturesData = removedFeaturesData;
        }

        public IList<IBackwardsEliminationRemovedFeatureData> RemovedFeaturesData { get; }
    }
}
