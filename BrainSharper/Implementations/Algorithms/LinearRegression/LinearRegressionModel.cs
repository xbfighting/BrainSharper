namespace BrainSharper.Implementations.Algorithms.LinearRegression
{
    using Abstract.Algorithms.LinearRegression;
    using MathNet.Numerics.LinearAlgebra;

    public class LinearRegressionModel : ILinearRegressionModel
    {
        public LinearRegressionModel(Vector<double> weights)
        {
            Weights = weights;
        }

        public Vector<double> Weights { get; }
    }
}
