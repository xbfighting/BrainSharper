using System.Collections.Generic;
using BrainSharper.Abstract.Algorithms.Infrastructure;
using MathNet.Numerics.LinearAlgebra;

namespace BrainSharper.Abstract.Algorithms.Knn
{
    public interface IKnnPredictionModel : IPredictionModel
    {
        Matrix<double> TrainingData { get; }
        IList<double> ExpectedTrainingOutcomes { get; }
        int KNeighbors { get; }
        bool UseWeightedDistance { get; }
    }
}
