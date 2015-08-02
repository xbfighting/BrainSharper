namespace BrainSharper.Implementations.Algorithms.DecisionTrees.Processors
{
    using System;
    using System.Collections.Generic;
    using System.Linq;

    using Abstract.Algorithms.DecisionTrees.DataStructures;
    using Abstract.Algorithms.DecisionTrees.Processors;
    using Abstract.Data;

    using DataStructures.BinaryDecisionTrees;

    using DataStructures;

    using General.Utils;

    public class MultiValueSplitSelectorForCategoricalOutcome<TValue> : IBestSplitSelector
    {
        private readonly IDataSplitter<TValue> categoricalDataSplitter;
        private readonly IBinaryNumericDataSplitter binaryNumericDataSplitter;

        public MultiValueSplitSelectorForCategoricalOutcome(IDataSplitter<TValue> categoricalSplitter, IBinaryNumericDataSplitter binarySplitter)
        {
            this.categoricalDataSplitter = categoricalSplitter;
            this.binaryNumericDataSplitter = binarySplitter;
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
                    var rowsByAttributeValues =
                        baseData.GetNumericColumnVector(attributeToSplit)
                            .Select((val, index) => new { Value = val, Index = index, DependentAttrVal = baseData[index, dependentFeatureName], Row = baseData.GetRowVector(index) })
                            .GroupBy(elem => elem.Value)
                            .Select(grp => new { FeatureValue = grp.Key, SortedIndices = grp.OrderBy(elem => elem.Index).ToList() })
                            .OrderBy(grp => grp.FeatureValue)
                            .ToList();
                    var elemsSoFar = new List<Tuple<double, object>>();
                    var previousClass = rowsByAttributeValues[0].SortedIndices[0].DependentAttrVal;
                    var previousValue = Convert.ToDouble(rowsByAttributeValues[0].SortedIndices[0].Value);
                    foreach (var groupAndExamples in rowsByAttributeValues)
                    {
                        var currentValue = groupAndExamples.FeatureValue;
                        bool tieFound = false;
                        foreach (var rowData in groupAndExamples.SortedIndices)
                        {
                            var currentRowIndex = rowData.Index;
                            var currentClass = rowData.DependentAttrVal;
                            elemsSoFar.Add(new Tuple<double, object>(groupAndExamples.FeatureValue, currentClass));
                            if (!currentClass.Equals(previousClass))
                            {
                                if (previousValue == currentValue)
                                {
                                    previousClass = currentClass;
                                    tieFound = true;
                                    break;
                                }
                                var halfWay = (currentValue + previousValue) / 2.0;
                                var splitParams = new BinarySplittingParams<double>(
                                    attributeToSplit,
                                    halfWay,
                                    dependentFeatureName);
                                IList<ISplittedData> splittedData = binaryNumericDataSplitter.SplitData(baseData, splitParams);
                                double splitQuality = splitQualityChecker.CalculateSplitQuality(
                                    initialEntropy,
                                    totalRowsCount,
                                    splittedData,
                                    dependentFeatureName);
                                if (splitQuality > bestSplitQuality)
                                {
                                    bestSplitQuality = splitQuality;
                                    bestSplit = new BinarySplittingResult(true, attributeToSplit, splittedData, halfWay);
                                }
                                previousClass = currentClass;
                            }
                            previousValue = currentValue;
                        }
                        if (tieFound)
                        {
                            var rowValue = groupAndExamples.SortedIndices.Last().Value;
                            previousClass = groupAndExamples.SortedIndices.Last().DependentAttrVal;
                            previousValue = rowValue;
                        }
                    }
                }
                else
                {
                    var allPossibleValues = baseData.GetColumnVector<TValue>(attributeToSplit).Distinct();
                    foreach (var possibleValue in allPossibleValues)
                    {
                        var splitParam = new SplittingParams<TValue>(attributeToSplit, dependentFeatureName);
                        var splittedData = this.categoricalDataSplitter.SplitData(baseData, splitParam);
                        var splitQuality = splitQualityChecker.CalculateSplitQuality(
                            initialEntropy,
                            totalRowsCount,
                            splittedData,
                            dependentFeatureName);
                        if (splitQuality > bestSplitQuality)
                        {
                            bestSplitQuality = splitQuality;
                            bestSplit = new SplittingResult<TValue>(false, attributeToSplit, splittedData);
                        }
                    }
                }
            }
            return bestSplit;
        }
    }
}
