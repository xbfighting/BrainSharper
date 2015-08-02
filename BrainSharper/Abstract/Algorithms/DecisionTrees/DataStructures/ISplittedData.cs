namespace BrainSharper.Abstract.Algorithms.DecisionTrees.DataStructures
{
    using Data;

    public interface ISplittedData
    {
        IDecisionTreeLink SplitLink { get; }
        IDataFrame SplittedDataFrame { get; }
    }
}
