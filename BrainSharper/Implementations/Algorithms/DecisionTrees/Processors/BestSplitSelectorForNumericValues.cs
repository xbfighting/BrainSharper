using System;
using System.Linq;
using BrainSharper.Abstract.Algorithms.DecisionTrees.DataStructures;
using BrainSharper.Abstract.Algorithms.DecisionTrees.Processors;
using BrainSharper.Abstract.Data;
using BrainSharper.Implementations.Algorithms.DecisionTrees.DataStructures;
using BrainSharper.Implementations.Algorithms.DecisionTrees.DataStructures.BinaryDecisionTrees;

namespace BrainSharper.Implementations.Algorithms.DecisionTrees.Processors
{
    public class BestSplitSelectorForNumericValues : IBestSplitSelectorForNumericOutcome
    {
        private readonly IBinaryNumericDataSplitter binaryNumericDataSplitter;

        public BestSplitSelectorForNumericValues(IBinaryNumericDataSplitter binaryNumericSplitter)
        {
            binaryNumericDataSplitter = binaryNumericSplitter;
        }

        public ISplittingResult SelectBestSplit(
            IDataFrame baseData,
            string dependentFeatureName,
            ISplitQualityChecker splitQualityChecker,
            IAlredyUsedAttributesInfo alreadyUsedAttributesInfo)
        {
            if (!(splitQualityChecker is INumericalSplitQualityChecker))
            {
                throw new ArgumentException("Invalid split quality checker for numerical outcome");
            }
            return SelectBestSplit(baseData, dependentFeatureName, (INumericalSplitQualityChecker) splitQualityChecker,
                alreadyUsedAttributesInfo);
        }

        public ISplittingResult SelectBestSplit(
            IDataFrame baseData,
            string dependentFeatureName,
            INumericalSplitQualityChecker splitQualityChecker,
            IAlredyUsedAttributesInfo alreadyUsedAttributesInfo)
        {
            var bestSplitQuality = double.NegativeInfinity;
            var initialEntropy = splitQualityChecker.GetInitialEntropy(baseData, dependentFeatureName);
            Tuple<string, double> bestSplit = null;

            var featureColumns = baseData.ColumnNames.Except(new[] {dependentFeatureName});
            foreach (var feature in featureColumns)
            {
                var dataOrderedByFeature =
                    baseData.GetNumericColumnVector(feature)
                        .Select(
                            (rowVal, idx) =>
                                new Tuple<double, double, int>(rowVal,
                                    (double) baseData[idx, dependentFeatureName].FeatureValue, idx))
                        .OrderBy(tpl => tpl.Item1)
                        .ToList();
                var dependentFeatureValuesOrdered = dataOrderedByFeature.Select(elem => elem.Item2).ToList();

                var previousFeatureValue = dataOrderedByFeature.First().Item1;

                for (var i = 0; i < (dataOrderedByFeature.Count - 1); i++)
                {
                    var dataPoint = dataOrderedByFeature[i];
                    var currentFeatureValue = dataPoint.Item1;
                    if (currentFeatureValue != previousFeatureValue)
                    {
                        var splitPoint = (currentFeatureValue + previousFeatureValue)/2.0;
                        if (!alreadyUsedAttributesInfo.WasAttributeAlreadyUsedWithValue(feature, splitPoint))
                        {
                            var dependentValsBelow = dependentFeatureValuesOrdered.Take(i).ToList();
                            var dependentValsAbove = dependentFeatureValuesOrdered.Skip(i).ToList();
                            var splitQuality = splitQualityChecker.CalculateSplitQuality(
                                initialEntropy,
                                baseData.RowCount,
                                new[] {dependentValsBelow, dependentValsAbove});
                            if (splitQuality > bestSplitQuality)
                            {
                                bestSplitQuality = splitQuality;
                                bestSplit = new Tuple<string, double>(feature, splitPoint);
                            }
                        }
                    }

                    previousFeatureValue = currentFeatureValue;
                }
            }

            if (bestSplit == null)
            {
                return null;
            }

            var splittedData = binaryNumericDataSplitter.SplitData(
                baseData,
                new BinarySplittingParams(bestSplit.Item1, bestSplit.Item2, dependentFeatureName));

            return new BinarySplittingResult(true, bestSplit.Item1, splittedData, bestSplit.Item2);
        }
    }
}