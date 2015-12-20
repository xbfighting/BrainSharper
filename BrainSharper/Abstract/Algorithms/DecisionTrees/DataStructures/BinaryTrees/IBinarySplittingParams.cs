namespace BrainSharper.Abstract.Algorithms.DecisionTrees.DataStructures.BinaryTrees
{
    public interface IBinarySplittingParams : ISplittingParams
    {
        object SplitOnValue { get; }
    }
}