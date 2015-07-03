using System.Collections.Generic;
using BrainSharper.Abstract.Algorithms.Knn;
using MathNet.Numerics.LinearAlgebra;

namespace BrainSharper.Implementations.Algorithms.Knn
{
    public class BackwardsEliminationKnnModel : KnnPredictionModel, IBackwardsEliminationKnnModel
    {
        public BackwardsEliminationKnnModel(
            Matrix<double> trainingData,
            IList<double> expectedTrainingOutcomes,
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
