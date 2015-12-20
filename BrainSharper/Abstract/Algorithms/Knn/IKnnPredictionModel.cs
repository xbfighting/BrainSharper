using System.Collections.Generic;
using BrainSharper.Abstract.Algorithms.Infrastructure;
using MathNet.Numerics.LinearAlgebra;

namespace BrainSharper.Abstract.Algorithms.Knn
{
    public interface IKnnPredictionModel<TPredictionResult> : IPredictionModel
    {
        Matrix<double> TrainingData { get; }
        IList<TPredictionResult> ExpectedTrainingOutcomes { get; }
        int KNeighbors { get; }
        bool UseWeightedDistance { get; }
        IList<string> DataColumnsNames { get; }
    }
}