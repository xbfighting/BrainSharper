namespace BrainSharper.Abstract.Algorithms.DecisionTrees.Processors
{
    using System;

    using Data;

    using DataStructures;

    public interface IBinaryNumericAttributeBestSplitPointSelector
    {
        Tuple<ISplittingResult, double> FindBestSplitPoint(
            IDataFrame baseData,
            string dependentFeatureName,
            string numericFeatureToProcess,
            ISplitQualityChecker splitQualityChecker,
            IBinaryNumericDataSplitter binaryNumericDataSplitter,
            double initialEntropy);
    }
}
