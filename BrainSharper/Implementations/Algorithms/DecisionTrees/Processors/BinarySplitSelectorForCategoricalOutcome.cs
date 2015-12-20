using System;
using System.Collections.Generic;
using System.Linq;
using BrainSharper.Abstract.Algorithms.DecisionTrees.DataStructures;
using BrainSharper.Abstract.Algorithms.DecisionTrees.DataStructures.BinaryTrees;
using BrainSharper.Abstract.Algorithms.DecisionTrees.Processors;
using BrainSharper.Abstract.Data;
using BrainSharper.Implementations.Algorithms.DecisionTrees.DataStructures;
using BrainSharper.Implementations.Algorithms.DecisionTrees.DataStructures.BinaryDecisionTrees;

namespace BrainSharper.Implementations.Algorithms.DecisionTrees.Processors
{
    public class BinarySplitSelectorForCategoricalOutcome : BaseSplitSelectorForCategoricalOutcome,
        IBinaryBestSplitSelector
    {
        public BinarySplitSelectorForCategoricalOutcome(
            IBinaryDataSplitter binaryDataSplitter,
            IBinaryNumericDataSplitter binaryNumericDataSplitter,
            IBinaryNumericAttributeSplitPointSelector binaryNumericBestSplitPointSelector)
            : base(binaryDataSplitter, binaryNumericDataSplitter, binaryNumericBestSplitPointSelector)
        {
        }

        protected override Tuple<IList<ISplittedData>, ISplittingParams, double> EvaluateCategoricalSplit(
            IDataFrame dataToSplit,
            string dependentFeatureName,
            string splittingFeatureName,
            double bestSplitQualitySoFar,
            double initialEntropy,
            ISplitQualityChecker splitQualityChecker,
            IAlredyUsedAttributesInfo alredyUsedAttributesInfo)
        {
            var totalRowsCount = dataToSplit.RowCount;
            var uniqueFeatureValues = dataToSplit.GetColumnVector(splittingFeatureName).Distinct();
            var locallyBestSplitQuality = double.NegativeInfinity;
            IBinarySplittingParams localBestSplitParams = null;
            IList<ISplittedData> locallyBestSplitData = null;
            foreach (var featureValue in uniqueFeatureValues)
            {
                if (!alredyUsedAttributesInfo.WasAttributeAlreadyUsedWithValue(splittingFeatureName, featureValue))
                {
                    var binarySplitParams = new BinarySplittingParams(splittingFeatureName, featureValue,
                        dependentFeatureName);
                    var splittedData = CategoricalDataSplitter.SplitData(dataToSplit, binarySplitParams);
                    if (splittedData.Count == 1)
                    {
                        return new Tuple<IList<ISplittedData>, ISplittingParams, double>(
                            new List<ISplittedData>(),
                            binarySplitParams,
                            double.NegativeInfinity);
                    }

                    var splitQuality = splitQualityChecker.CalculateSplitQuality(
                        initialEntropy,
                        totalRowsCount,
                        splittedData,
                        dependentFeatureName);
                    if (splitQuality > locallyBestSplitQuality)
                    {
                        locallyBestSplitQuality = splitQuality;
                        locallyBestSplitData = splittedData;
                        localBestSplitParams = binarySplitParams;
                    }
                }
            }

            return new Tuple<IList<ISplittedData>, ISplittingParams, double>(
                locallyBestSplitData,
                localBestSplitParams,
                locallyBestSplitQuality);
        }

        protected override ISplittingResult BuildBestSplitObject(ISplittingParams splittingParams,
            IList<ISplittedData> splittedData)
        {
            var binarySplittingParams = splittingParams as IBinarySplittingParams;
            if (binarySplittingParams == null)
            {
                throw new ArgumentException("Invalid type of splitting params passed to binary split selector!");
            }
            return new BinarySplittingResult(false, binarySplittingParams.SplitOnFeature, splittedData,
                binarySplittingParams.SplitOnValue);
        }

        protected override void UpdateAlreadyUsedAttributes(ISplittingParams splittingParams,
            IAlredyUsedAttributesInfo alreadyUsedAttributesInfo)
        {
            var binarySplittingParams = splittingParams as IBinarySplittingParams;
            alreadyUsedAttributesInfo.AddAlreadyUsedAttribute(splittingParams.SplitOnFeature,
                binarySplittingParams.SplitOnValue);
        }
    }
}