using System;
using System.Collections.Generic;
using System.Linq;
using BrainSharper.Abstract.Algorithms.DecisionTrees.DataStructures;
using BrainSharper.Abstract.Algorithms.DecisionTrees.Processors;
using BrainSharper.Abstract.Data;
using BrainSharper.General.Utils;

namespace BrainSharper.Implementations.Algorithms.DecisionTrees.Processors
{
    public abstract class BaseSplitSelectorForCategoricalOutcome : IBestSplitSelector
    {
        protected readonly IBinaryNumericAttributeSplitPointSelector BinaryNumericBestSplitingPointSelector;
        protected readonly IBinaryNumericDataSplitter BinaryNumericDataSplitter;
        protected readonly IDataSplitter CategoricalDataSplitter;

        protected BaseSplitSelectorForCategoricalOutcome(
            IDataSplitter binarySplitter,
            IBinaryNumericDataSplitter binaryNumericSplitter,
            IBinaryNumericAttributeSplitPointSelector binaryNumericBestSplitPointSelector)
        {
            CategoricalDataSplitter = binarySplitter;
            BinaryNumericDataSplitter = binaryNumericSplitter;
            BinaryNumericBestSplitingPointSelector = binaryNumericBestSplitPointSelector;
        }

        public ISplittingResult SelectBestSplit(
            IDataFrame baseData,
            string dependentFeatureName,
            ISplitQualityChecker splitQualityChecker,
            IAlredyUsedAttributesInfo alreadyUsedAttributesInfo)
        {
            ISplittingResult bestSplit = null;
            double bestSplitQuality = float.NegativeInfinity;
            var initialEntropy = splitQualityChecker.GetInitialEntropy(baseData, dependentFeatureName);

            foreach (var attributeToSplit in baseData.ColumnNames.Except(new[] {dependentFeatureName}))
            {
                if (baseData.GetColumnType(attributeToSplit).TypeIsNumeric())
                {
                    // TODO: add checking for the already used attribtues
                    var bestNumericSplitPointAndQuality =
                        BinaryNumericBestSplitingPointSelector.FindBestSplitPoint(
                            baseData,
                            dependentFeatureName,
                            attributeToSplit,
                            splitQualityChecker,
                            BinaryNumericDataSplitter,
                            initialEntropy);
                    if (bestNumericSplitPointAndQuality.Item2 > bestSplitQuality)
                    {
                        bestSplitQuality = bestNumericSplitPointAndQuality.Item2;
                        bestSplit = bestNumericSplitPointAndQuality.Item1;
                    }
                }
                else
                {
                    var bestSplitForAttribute = EvaluateCategoricalSplit(
                        baseData,
                        dependentFeatureName,
                        attributeToSplit,
                        bestSplitQuality,
                        initialEntropy,
                        splitQualityChecker,
                        alreadyUsedAttributesInfo);
                    if (bestSplitForAttribute.Item3 > bestSplitQuality)
                    {
                        bestSplit = BuildBestSplitObject(bestSplitForAttribute.Item2, bestSplitForAttribute.Item1);
                        bestSplitQuality = bestSplitForAttribute.Item3;
                    }
                }
            }
            return bestSplit;
        }

        //TODO: AAA make it nicer - maybe encapsulate Tuple in some dto
        protected abstract Tuple<IList<ISplittedData>, ISplittingParams, double> EvaluateCategoricalSplit(
            IDataFrame dataToSplit,
            string dependentFeatureName,
            string splittingFeatureName,
            double bestSplitQualitySoFar,
            double initialEntropy,
            ISplitQualityChecker splitQualityChecker,
            IAlredyUsedAttributesInfo alredyUsedAttributesInfo);

        protected abstract ISplittingResult BuildBestSplitObject(
            ISplittingParams splittingParams,
            IList<ISplittedData> splittedData);

        protected abstract void UpdateAlreadyUsedAttributes(
            ISplittingParams splittingParams,
            IAlredyUsedAttributesInfo alreadyUsedAttributesInfo);
    }
}