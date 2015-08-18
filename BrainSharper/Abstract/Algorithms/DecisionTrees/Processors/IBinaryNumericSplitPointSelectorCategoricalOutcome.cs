namespace BrainSharper.Abstract.Algorithms.DecisionTrees.Processors
{
    using System;

    using DataStructures;
    using Data;

    public interface IBinaryNumericSplitPointSelectorCategoricalOutcome : IBinaryNumericAttributeSplitPointSelector
    {
        Tuple<ISplittingResult, double> FindBestSplitPoint(
            IDataFrame baseData,
            string dependentFeatureName,
            string numericFeatureToProcess,
            ICategoricalSplitQualityChecker splitQualityChecker,
            IBinaryNumericDataSplitter binaryNumericDataSplitter,
            double initialEntropy);
    }
}
