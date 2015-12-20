using BrainSharper.Abstract.Algorithms.Infrastructure;

namespace BrainSharper.Abstract.Algorithms.LinearRegression
{
    public interface ILinearRegressionParams : IModelBuilderParams
    {
        double LearningRate { get; }
    }
}