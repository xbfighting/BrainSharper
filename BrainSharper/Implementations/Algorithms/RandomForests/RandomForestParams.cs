using BrainSharper.Abstract.Algorithms.RandomForests;

namespace BrainSharper.Implementations.Algorithms.RandomForests
{
    public class RandomForestParams : IRandomForestModelBuilderParams
    {
        public RandomForestParams(int treesCount, int maximalTreeDepth)
        {
            TreesCount = treesCount;
            MaximalTreeDepth = maximalTreeDepth;
        }

        public int TreesCount { get; }
        public int MaximalTreeDepth { get; }
    }
}