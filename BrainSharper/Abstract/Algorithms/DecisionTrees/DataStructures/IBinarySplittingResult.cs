namespace BrainSharper.Abstract.Algorithms.DecisionTrees.DataStructures
{
    public interface IBinarySplittingResult : ISplittingResult
    {
        object SplittingValue { get; }
    }
}
