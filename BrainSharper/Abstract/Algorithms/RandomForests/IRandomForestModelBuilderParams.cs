namespace BrainSharper.Abstract.Algorithms.RandomForests
{
    using Infrastructure;

    public interface IRandomForestModelBuilderParams : IModelBuilderParams
    {
        int TreesCount { get; }
        int MaximalTreeDepth { get; }
    }
}
