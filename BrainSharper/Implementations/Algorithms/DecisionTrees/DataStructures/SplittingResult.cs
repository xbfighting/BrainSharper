using BrainSharper.Abstract.Algorithms.DecisionTrees.DataStructures;
using BrainSharper.Abstract.Data;

namespace BrainSharper.Implementations.Algorithms.DecisionTrees.DataStructures
{
    public class SplittingResult : ISplittingResult
    {
        public SplittingResult(IDecisionTreeLink splitLink, IDataFrame splittedDataFrame)
        {
            SplitLink = splitLink;
            SplittedDataFrame = splittedDataFrame;
        }

        public IDecisionTreeLink SplitLink { get; }
        public IDataFrame SplittedDataFrame { get; }
    }
}
