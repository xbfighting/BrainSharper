using System.Collections.Generic;
using BrainSharper.Abstract.Algorithms.DecisionTrees.DataStructures;

namespace BrainSharper.Implementations.Algorithms.DecisionTrees.DataStructures
{
    public class BinarySplittingResult : SplittingResult, IBinarySplittingResult
    {
        public BinarySplittingResult(
            bool isSplitNumeric, 
            string splittingFeatureName, 
            IList<ISplittedData> splittedDataSets, 
            object splittingValue) 
            : base(isSplitNumeric, splittingFeatureName, splittedDataSets)
        {
            SplittingValue = splittingValue;
        }

        public object SplittingValue { get; }
    }
}
