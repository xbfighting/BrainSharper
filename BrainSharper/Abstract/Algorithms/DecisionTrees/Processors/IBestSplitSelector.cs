using BrainSharper.Abstract.Algorithms.DecisionTrees.DataStructures;
using BrainSharper.Abstract.Data;

namespace BrainSharper.Abstract.Algorithms.DecisionTrees.Processors
{
    public interface IBestSplitSelector
    {
        // TODO: replace all IList<SplittedDataSets> with some single object wrapping it
        ISplittingResult SelectBestSplit(IDataFrame baseData, string dependentFeatureName, ISplitQualityChecker splitQualityChecker);
    }
}
