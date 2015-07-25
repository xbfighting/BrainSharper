namespace BrainSharper.Implementations.Algorithms.DecisionTrees.DataStructures
{
    using Abstract.Data;
    using Abstract.Algorithms.DecisionTrees.DataStructures;

    public class SplittedData<TTestResult> : ISplittedData<TTestResult>
    {
        public SplittedData(IDecisionTreeLink<TTestResult> splitLink, IDataFrame splittedDataFrame)
        {
            this.SplitLink = splitLink;
            this.SplittedDataFrame = splittedDataFrame;
        }

        public IDecisionTreeLink<TTestResult> SplitLink { get; }
        
        public IDataFrame SplittedDataFrame { get; }
    }
}
