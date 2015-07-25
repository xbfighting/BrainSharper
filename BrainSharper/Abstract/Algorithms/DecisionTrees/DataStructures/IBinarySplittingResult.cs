namespace BrainSharper.Abstract.Algorithms.DecisionTrees.DataStructures
{
    public interface IBinarySplittingResult : ISplittingResult<bool>
    {
        object SplittingValue { get; }
    }
}
