using System.Collections.Generic;
using BrainSharper.Abstract.Algorithms.Knn;
using MathNet.Numerics.LinearAlgebra;

namespace BrainSharper.Implementations.Algorithms.Knn
{
    public class KnnPredictionModel : IKnnPredictionModel
    {
        public KnnPredictionModel(Matrix<double> trainingData, IList<double> expectedTrainingOutcomes,  int kNeighbors, bool useWeightedDistance, double accuracy = 0)
        {
            TrainingData = trainingData;
            TrainingDataAccuracy = accuracy;
            KNeighbors = kNeighbors;
            UseWeightedDistance = useWeightedDistance;
            ExpectedTrainingOutcomes = expectedTrainingOutcomes;
        }

        public Matrix<double> TrainingData { get; }

        public IList<double> ExpectedTrainingOutcomes { get; }

        public int KNeighbors { get; }

        public bool UseWeightedDistance { get; }

        public double TrainingDataAccuracy { get; }
    }
}
