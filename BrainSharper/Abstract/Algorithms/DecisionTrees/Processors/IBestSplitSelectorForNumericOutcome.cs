using BrainSharper.Abstract.Algorithms.DecisionTrees.DataStructures;
using BrainSharper.Abstract.Data;
using BrainSharper.Implementations.Algorithms.DecisionTrees.Processors;

namespace BrainSharper.Abstract.Algorithms.DecisionTrees.Processors
{
    public interface IBestSplitSelectorForNumericOutcome : IBinaryBestSplitSelector
    {
        ISplittingResult SelectBestSplit(
            IDataFrame baseData,
            string dependentFeatureName,
            INumericalSplitQualityChecker splitQualityChecker,
            IAlredyUsedAttributesInfo alreadyUsedAttributesInfo);
    }
}