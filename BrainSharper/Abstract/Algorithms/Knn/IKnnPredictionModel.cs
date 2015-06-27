using BrainSharper.Abstract.Algorithms.Infrastructure;
using MathNet.Numerics.LinearAlgebra;

namespace BrainSharper.Abstract.Algorithms.Knn
{
    public interface IKnnPredictionModel : IPredictionModel
    {
        Matrix<double> Database { get; }
    }
}
