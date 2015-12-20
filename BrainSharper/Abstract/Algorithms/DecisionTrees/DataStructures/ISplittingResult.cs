using System.Collections.Generic;

namespace BrainSharper.Abstract.Algorithms.DecisionTrees.DataStructures
{
    public interface ISplittingResult
    {
        bool IsSplitNumeric { get; }
        string SplittingFeatureName { get; }
        IList<ISplittedData> SplittedDataSets { get; }
    }
}