namespace BrainSharper.Abstract.Algorithms.DecisionTrees.Processors
{
    using Data;

    using DataStructures;

    public interface IBestSplitSelector
    {
        ISplittingResult SelectBestSplit(IDataFrame baseData, string dependentFeatureName, ISplitQualityChecker splitQualityChecker);
    }
}
