﻿namespace BrainSharper.Implementations.Algorithms.DecisionTrees.Processors
{
    using System;
    using System.Collections.Generic;
    using System.Linq;

    using Abstract.Algorithms.DecisionTrees.DataStructures;
    using Abstract.Algorithms.DecisionTrees.Processors;
    using Abstract.Data;

    using General.Utils;

    public abstract class BaseSplitSelectorForCategoricalOutcome : IBestSplitSelector
    {
        protected readonly IDataSplitter CategoricalDataSplitter;
        protected readonly IBinaryNumericDataSplitter BinaryNumericDataSplitter;
        protected readonly IBinaryNumericAttributeBestSplitPointSelector BinaryNumericBestSplitingPointSelector;

        protected BaseSplitSelectorForCategoricalOutcome(
            IDataSplitter binarySplitter, 
            IBinaryNumericDataSplitter binaryNumericSplitter, 
            IBinaryNumericAttributeBestSplitPointSelector binaryNumericBestSplitPointSelector)
        {
            this.CategoricalDataSplitter = binarySplitter;
            this.BinaryNumericDataSplitter = binaryNumericSplitter;
            this.BinaryNumericBestSplitingPointSelector = binaryNumericBestSplitPointSelector;
        }

        public ISplittingResult SelectBestSplit(
            IDataFrame baseData,
            string dependentFeatureName,
            ISplitQualityChecker splitQualityChecker)
        {
            ISplittingResult bestSplit = null;
            double bestSplitQuality = float.NegativeInfinity;
            double initialEntropy = splitQualityChecker.GetInitialEntropy(baseData, dependentFeatureName);
            int totalRowsCount = baseData.RowCount;

            foreach (var attributeToSplit in baseData.ColumnNames.Except(new[] { dependentFeatureName }))
            {
                if (baseData.GetColumnType(attributeToSplit).TypeIsNumeric())
                {
                    var bestNumericSplitPointAndQuality =
                        this.BinaryNumericBestSplitingPointSelector.FindBestSplitPoint(
                            baseData,
                            dependentFeatureName,
                            attributeToSplit,
                            splitQualityChecker,
                            this.BinaryNumericDataSplitter,
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
                        splitQualityChecker);
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
            ISplitQualityChecker splitQualityChecker);

        protected abstract ISplittingResult BuildBestSplitObject(
            ISplittingParams splittingParams,
            IList<ISplittedData> splittedData);
    }
}
