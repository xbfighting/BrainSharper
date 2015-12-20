using System;
using System.Collections.Generic;
using BrainSharper.Abstract.Algorithms.DecisionTrees.DataStructures;
using BrainSharper.Abstract.Algorithms.DecisionTrees.Processors;
using BrainSharper.Abstract.Data;
using BrainSharper.Implementations.Algorithms.DecisionTrees.DataStructures;

namespace BrainSharper.Implementations.Algorithms.DecisionTrees.Processors
{
    public class MultiValueSplitSelectorForCategoricalOutcome : BaseSplitSelectorForCategoricalOutcome
    {
        public MultiValueSplitSelectorForCategoricalOutcome(
            IDataSplitter categoricalSplitter,
            IBinaryNumericDataSplitter binarySplitter,
            IBinaryNumericAttributeSplitPointSelector binaryNumericBestSplitPointSelector)
            : base(categoricalSplitter, binarySplitter, binaryNumericBestSplitPointSelector)
        {
        }

        protected override Tuple<IList<ISplittedData>, ISplittingParams, double> EvaluateCategoricalSplit(
            IDataFrame dataToSplit,
            string dependentFeatureName,
            string splittingFeatureName,
            double bestSplitQualitySoFar,
            double initialEntropy,
            ISplitQualityChecker splitQualityChecker,
            IAlredyUsedAttributesInfo alreadyUsedAttributesInfo)
        {
            if (alreadyUsedAttributesInfo.WasAttributeAlreadyUsed(splittingFeatureName))
            {
                return new Tuple<IList<ISplittedData>, ISplittingParams, double>(
                    new List<ISplittedData>(),
                    new SplittingParams(splittingFeatureName, dependentFeatureName),
                    double.NegativeInfinity);
            }
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

            var splitQuality = splitQualityChecker.CalculateSplitQuality(initialEntropy, totalRowsCount, splitData,
                dependentFeatureName);
            return new Tuple<IList<ISplittedData>, ISplittingParams, double>(splitData, splitParams, splitQuality);
        }

        protected override ISplittingResult BuildBestSplitObject(ISplittingParams splittingParams,
            IList<ISplittedData> splittedData)
        {
            return new SplittingResult(
                false,
                splittingParams.SplitOnFeature,
                splittedData);
        }

        protected override void UpdateAlreadyUsedAttributes(ISplittingParams splittingParams,
            IAlredyUsedAttributesInfo alreadyUsedAttributesInfo)
        {
            alreadyUsedAttributesInfo.AddAlreadyUsedAttribute(splittingParams.SplitOnFeature);
        }
    }
}