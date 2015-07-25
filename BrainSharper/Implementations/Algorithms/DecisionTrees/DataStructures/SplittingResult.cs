using System.Collections.Generic;
using BrainSharper.Abstract.Algorithms.DecisionTrees.DataStructures;

namespace BrainSharper.Implementations.Algorithms.DecisionTrees.DataStructures
{
    public class SplittingResult<TTestResult> : ISplittingResult<TTestResult>
    {
        public SplittingResult(bool isSplitNumeric, string splittingFeatureName, IList<ISplittedData<TTestResult>> splittedDataSets)
        {
            IsSplitNumeric = isSplitNumeric;
            SplittingFeatureName = splittingFeatureName;
            SplittedDataSets = splittedDataSets;
        }

        public bool IsSplitNumeric { get; }
        public string SplittingFeatureName { get; }
        public IList<ISplittedData<TTestResult>> SplittedDataSets { get; }
    }
}
