namespace BrainSharper.Abstract.Algorithms.LinearRegression
{
    using Infrastructure;

    public interface ILinearRegressionParams : IModelBuilderParams
    {
        double LearningRate { get; }
    }
}
