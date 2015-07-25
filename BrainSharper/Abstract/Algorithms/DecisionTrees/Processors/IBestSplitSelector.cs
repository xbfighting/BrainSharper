using BrainSharper.Abstract.Algorithms.DecisionTrees.DataStructures;
using BrainSharper.Abstract.Data;

namespace BrainSharper.Abstract.Algorithms.DecisionTrees.Processors
{
    public interface IBestSplitSelector
    {
        ISplittingResult SelectBestSplit(IDataFrame baseData, string dependentFeatureName, ISplitQualityChecker splitQualityChecker);
    }
}
