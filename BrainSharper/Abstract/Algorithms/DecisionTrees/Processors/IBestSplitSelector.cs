using BrainSharper.Abstract.Algorithms.DecisionTrees.DataStructures;
using BrainSharper.Abstract.Data;

namespace BrainSharper.Abstract.Algorithms.DecisionTrees.Processors
{
    public interface IBestSplitSelector<TTestResult>
    {
        ISplittingResult<TTestResult> SelectBestSplit(IDataFrame baseData, string dependentFeatureName, ISplitQualityChecker<TTestResult> splitQualityChecker);
    }
}
