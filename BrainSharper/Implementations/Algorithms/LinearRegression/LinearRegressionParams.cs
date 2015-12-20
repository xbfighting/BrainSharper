using BrainSharper.Abstract.Algorithms.LinearRegression;

namespace BrainSharper.Implementations.Algorithms.LinearRegression
{
    public class LinearRegressionParams : ILinearRegressionParams
    {
        public LinearRegressionParams(double learningRate)
        {
            LearningRate = learningRate;
        }

        public double LearningRate { get; }
    }
}