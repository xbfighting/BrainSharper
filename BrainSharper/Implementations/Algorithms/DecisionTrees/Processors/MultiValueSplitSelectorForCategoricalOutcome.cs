namespace BrainSharper.Implementations.Algorithms.DecisionTrees.Processors
{
    using System;
    using System.Collections.Generic;

    using Abstract.Algorithms.DecisionTrees.DataStructures;
    using Abstract.Algorithms.DecisionTrees.Processors;

    using Abstract.Data;

    using DataStructures;

    public class MultiValueSplitSelectorForCategoricalOutcome : BaseSplitSelectorForCategoricalOutcome
    {
        public MultiValueSplitSelectorForCategoricalOutcome(
            IDataSplitter categoricalSplitter, 
            IBinaryNumericDataSplitter binarySplitter,
            IBinaryNumericAttributeBestSplitPointSelector binaryNumericBestSplitPointSelector)
            : base(categoricalSplitter, binarySplitter, binaryNumericBestSplitPointSelector)
        {
        }

        protected override Tuple<IList<ISplittedData>, ISplittingParams, double> EvaluateCategoricalSplit(
            IDataFrame dataToSplit,
            string dependentFeatureName,
            string splittingFeatureName,
            double bestSplitQualitySoFar,
            double initialEntropy,
            ISplitQualityChecker splitQualityChecker)
        {
            var totalRowsCount = dataToSplit.RowCount;
            var splitParams = new SplittingParams(splittingFeatureName, dependentFeatureName);
            var splitData = CategoricalDataSplitter.SplitData(dataToSplit, splitParams);
            if (splitData.Count == 1)
            {
                return new Tuple<IList<ISplittedData>, ISplittingParams, double>(
                    new List<ISplittedData>(), 
                    splitParams, 
                    double.NegativeInfinity);
            }

            var splitQuality = splitQualityChecker.CalculateSplitQuality(initialEntropy, totalRowsCount, splitData, dependentFeatureName);
            return new Tuple<IList<ISplittedData>, ISplittingParams, double>(splitData, splitParams, splitQuality);
        }

        protected override ISplittingResult BuildBestSplitObject(ISplittingParams splittingParams, IList<ISplittedData> splittedData)
        {
            return new SplittingResult(
                false,
                splittingParams.SplitOnFeature,
                splittedData);
        }
    }
}
