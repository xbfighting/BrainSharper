using BrainSharper.Abstract.Algorithms.LinearRegression;
using MathNet.Numerics.LinearAlgebra;

namespace BrainSharper.Implementations.Algorithms.LinearRegression
{
    public class LinearRegressionModel : ILinearRegressionModel
    {
        public LinearRegressionModel(Vector<double> weights)
        {
            Weights = weights;
        }

        public Vector<double> Weights { get; }
    }
}