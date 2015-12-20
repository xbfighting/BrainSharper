using BrainSharper.Abstract.Algorithms.Infrastructure;
using MathNet.Numerics.LinearAlgebra;

namespace BrainSharper.Abstract.Algorithms.LinearRegression
{
    public interface ILinearRegressionModelBuilder : IPredictionModelBuilder
    {
        ILinearRegressionModel BuildModel(
            Matrix<double> matrixX,
            Vector<double> vectorY,
            ILinearRegressionParams linearRegressionParams);
    }
}