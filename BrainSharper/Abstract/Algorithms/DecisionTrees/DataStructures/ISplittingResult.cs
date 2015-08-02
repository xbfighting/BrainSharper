namespace BrainSharper.Abstract.Algorithms.DecisionTrees.DataStructures
{
    using System.Collections.Generic;

    public interface ISplittingResult
    {
        bool IsSplitNumeric { get; }
        string SplittingFeatureName { get; }
        IList<ISplittedData> SplittedDataSets { get; }
    }
}
