using BrainSharper.Abstract.Data;

namespace BrainSharper.Abstract.Algorithms.DecisionTrees.DataStructures
{
    public interface ISplittedData<TTestResult>
    {
        IDecisionTreeLink<TTestResult> SplitLink { get; }
        IDataFrame SplittedDataFrame { get; }
    }
}
