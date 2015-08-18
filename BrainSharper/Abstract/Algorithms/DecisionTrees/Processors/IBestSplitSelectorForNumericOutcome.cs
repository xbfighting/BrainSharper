namespace BrainSharper.Abstract.Algorithms.DecisionTrees.Processors
{
    using BrainSharper.Implementations.Algorithms.DecisionTrees.Processors;

    using Data;
    using DataStructures;

    public interface IBestSplitSelectorForNumericOutcome : IBinaryBestSplitSelector
    {
        ISplittingResult SelectBestSplit(
            IDataFrame baseData,
            string dependentFeatureName,
            INumericalSplitQualityChecker splitQualityChecker,
            IAlredyUsedAttributesInfo alreadyUsedAttributesInfo);
    }
}
