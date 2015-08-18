namespace BrainSharper.Abstract.Algorithms.DecisionTrees.Processors
{
    using System;

    using Data;

    using DataStructures;

    public interface IBinaryNumericAttributeSplitPointSelector
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
