namespace BrainSharper.Implementations.Algorithms.DecisionTrees.DataStructures
{
    using Abstract.Algorithms.DecisionTrees.DataStructures;
    using Abstract.Data;

    public class SplittedData<TTestResult> : ISplittedData
    {
        public SplittedData(IDecisionTreeLink splitLink, IDataFrame splittedDataFrame)
        {
            this.SplitLink = splitLink;
            this.SplittedDataFrame = splittedDataFrame;
        }

        public IDecisionTreeLink SplitLink { get; }
        
        public IDataFrame SplittedDataFrame { get; }
    }
}
