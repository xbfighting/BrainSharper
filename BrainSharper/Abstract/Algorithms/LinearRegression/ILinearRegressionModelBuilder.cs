namespace BrainSharper.Abstract.Algorithms.LinearRegression
{
    using Infrastructure;

    using MathNet.Numerics.LinearAlgebra;

    public interface ILinearRegressionModelBuilder : IPredictionModelBuilder
    {
        ILinearRegressionModel BuildModel(
            Matrix<double> matrixX,
            Vector<double> vectorY,
            ILinearRegressionParams linearRegressionParams);
    }
}
