using BrainSharper.Abstract.Data;

namespace BrainSharper.Abstract.Algorithms.DecisionTrees.DataStructures
{
    public interface ISplittingResult
    {
        IDecisionTreeLink SplitLink { get; }
        IDataFrame SplittedDataFrame { get; }
    }
}
