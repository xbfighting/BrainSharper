namespace BrainSharper.Abstract.Algorithms.DecisionTrees.DataStructures.BinaryTrees
{
    public interface IBinarySplittingParams<TSplittingValueType> : ISplittingParams
    {
        TSplittingValueType SplitOnValue { get; } 
    }
}
