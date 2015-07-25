using System.Collections.Generic;

namespace BrainSharper.Abstract.Algorithms.DecisionTrees.DataStructures
{
    public interface ISplittingResult<TTestResult>
    {
        bool IsSplitNumeric { get; }
        string SplittingFeatureName { get; }
        IList<ISplittedData<TTestResult>> SplittedDataSets { get; }
    }
}
