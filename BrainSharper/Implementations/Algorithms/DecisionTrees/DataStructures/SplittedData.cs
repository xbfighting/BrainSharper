using BrainSharper.Abstract.Algorithms.DecisionTrees.DataStructures;
using BrainSharper.Abstract.Data;

namespace BrainSharper.Implementations.Algorithms.DecisionTrees.DataStructures
{
    public class SplittedData : ISplittedData
    {
        public SplittedData(IDecisionTreeLink splitLink, IDataFrame splittedDataFrame)
        {
            SplitLink = splitLink;
            SplittedDataFrame = splittedDataFrame;
        }

        public IDecisionTreeLink SplitLink { get; }
        public IDataFrame SplittedDataFrame { get; }
    }
}