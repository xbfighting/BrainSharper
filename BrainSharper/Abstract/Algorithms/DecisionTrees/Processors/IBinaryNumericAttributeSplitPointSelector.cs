using System;
using BrainSharper.Abstract.Algorithms.DecisionTrees.DataStructures;
using BrainSharper.Abstract.Data;

namespace BrainSharper.Abstract.Algorithms.DecisionTrees.Processors
{
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