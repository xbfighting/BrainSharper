namespace BrainSharper.Implementations.Algorithms.LinearRegression
{
    using BrainSharper.Abstract.Algorithms.LinearRegression;

    public class LinearRegressionParams : ILinearRegressionParams
    {
        public LinearRegressionParams(double learningRate)
        {
            this.LearningRate = learningRate;
        }

        public double LearningRate { get; }
    }
}
