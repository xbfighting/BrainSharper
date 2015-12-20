using BrainSharper.Abstract.Algorithms.Infrastructure;
using MathNet.Numerics.LinearAlgebra;

namespace BrainSharper.Abstract.Algorithms.LinearRegression
{
    public interface ILinearRegressionModel : IPredictionModel
    {
        Vector<double> Weights { get; }
    }
}