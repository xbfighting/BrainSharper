using BrainSharper.Abstract.Data;

namespace BrainSharper.Abstract.Algorithms.DecisionTrees.DataStructures
{
    public interface ISplittedData
    {
        IDecisionTreeLink SplitLink { get; }
        IDataFrame SplittedDataFrame { get; }
    }
}