using BrainSharper.Abstract.Algorithms.Infrastructure;

namespace BrainSharper.Abstract.Algorithms.RandomForests
{
    public interface IRandomForestModelBuilderParams : IModelBuilderParams
    {
        int TreesCount { get; }
        int MaximalTreeDepth { get; }
    }
}