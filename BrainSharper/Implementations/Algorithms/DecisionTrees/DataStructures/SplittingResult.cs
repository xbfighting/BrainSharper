using System.Collections.Generic;
using BrainSharper.Abstract.Algorithms.DecisionTrees.DataStructures;

namespace BrainSharper.Implementations.Algorithms.DecisionTrees.DataStructures
{
    public class SplittingResult : ISplittingResult
    {
        public SplittingResult(bool isSplitNumeric, string splittingFeatureName, IList<ISplittedData> splittedDataSets)
        {
            IsSplitNumeric = isSplitNumeric;
            SplittingFeatureName = splittingFeatureName;
            SplittedDataSets = splittedDataSets;
        }

        public bool IsSplitNumeric { get; }
        public string SplittingFeatureName { get; }
        public IList<ISplittedData> SplittedDataSets { get; }
    }
}