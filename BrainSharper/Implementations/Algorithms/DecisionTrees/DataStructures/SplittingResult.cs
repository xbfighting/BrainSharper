namespace BrainSharper.Implementations.Algorithms.DecisionTrees.DataStructures
{
    using System.Collections.Generic;
    using Abstract.Algorithms.DecisionTrees.DataStructures;

    public class SplittingResult<TTestResult> : ISplittingResult
    {
        public SplittingResult(bool isSplitNumeric, string splittingFeatureName, IList<ISplittedData> splittedDataSets)
        {
            this.IsSplitNumeric = isSplitNumeric;
            this.SplittingFeatureName = splittingFeatureName;
            this.SplittedDataSets = splittedDataSets;
        }

        public bool IsSplitNumeric { get; }
        public string SplittingFeatureName { get; }
        public IList<ISplittedData> SplittedDataSets { get; }
    }
}
