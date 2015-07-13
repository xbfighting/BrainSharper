namespace BrainSharper.Abstract.Algorithms.DecisionTrees.DataStructures
{
    public interface IBinarySplittingParams<TSplittingValueType> : ISplittingParams
    {
        TSplittingValueType SplitOnValue { get; } 
    }
}
