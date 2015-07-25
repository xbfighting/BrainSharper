using BrainSharper.Abstract.Algorithms.DecisionTrees.DataStructures;
using BrainSharper.Abstract.Data;

namespace BrainSharper.Implementations.Algorithms.DecisionTrees.DataStructures
{
    public class SplittedData<TTestResult> : ISplittedData<TTestResult>
    {
        public SplittedData(IDecisionTreeLink<TTestResult> splitLink, IDataFrame splittedDataFrame)
        {
            SplitLink = splitLink;
            SplittedDataFrame = splittedDataFrame;
        }

        public IDecisionTreeLink<TTestResult> SplitLink { get; }
        public IDataFrame SplittedDataFrame { get; }
    }
}
