namespace BrainSharper.Abstract.Algorithms.LinearRegression
{
    using BrainSharper.Abstract.Algorithms.Infrastructure;

    using MathNet.Numerics.LinearAlgebra;

    public interface ILinearRegressionModel : IPredictionModel
    {
        Vector<double> Weights { get; }
    }
}
