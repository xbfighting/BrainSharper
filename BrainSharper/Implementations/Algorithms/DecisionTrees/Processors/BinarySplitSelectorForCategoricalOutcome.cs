using System;
using System.Collections.Generic;
using System.Linq;
using BrainSharper.Abstract.Algorithms.DecisionTrees.DataStructures;
using BrainSharper.Abstract.Algorithms.DecisionTrees.Processors;
using BrainSharper.Abstract.Data;
using BrainSharper.General.Utils;
using BrainSharper.Implementations.Algorithms.DecisionTrees.DataStructures;
using BrainSharper.Implementations.Algorithms.DecisionTrees.DataStructures.BinaryDecisionTrees;

namespace BrainSharper.Implementations.Algorithms.DecisionTrees.Processors
{
    public class BinarySplitSelector<TValue> : IBinaryBestSplitSelector
    {
        private readonly IBinaryDataSplitter<TValue> _binaryDataSplitter;
        private readonly IBinaryNumericDataSplitter _binaryNumericDataSplitter;

        public BinarySplitSelector(IBinaryDataSplitter<TValue> binaryDataSplitter, IBinaryNumericDataSplitter binaryNumericDataSplitter)
        {
            _binaryDataSplitter = binaryDataSplitter;
            _binaryNumericDataSplitter = binaryNumericDataSplitter;
        }

        public ISplittingResult<bool> SelectBestSplit(
            IDataFrame baseData, 
            string dependentFeatureName, 
            ISplitQualityChecker<bool> splitQualityChecker)
        {
            ISplittingResult<bool> bestSplit = null;
            double bestSplitQuality = float.NegativeInfinity;
            double initialEntropy = splitQualityChecker.GetInitialEntropy(baseData, dependentFeatureName);
            int totalRowsCount = baseData.RowCount;

            // TODO: refactor this method to be nicer
            // TODO: add support for excluding already used splits

            foreach (var attributeToSplit in baseData.ColumnNames.Except(new [] { dependentFeatureName }))
            {
                if (baseData.GetColumnType(attributeToSplit).TypeIsNumeric())
                {
                    var attributeValues = baseData
                        .GetNumericColumnVector(attributeToSplit)
                        .Select((val, index) => new { Value = val, Index = index })
                        .OrderBy(elem => elem.Value)
                        .ToList();
                    var previousClass = baseData[attributeValues[0].Index, dependentFeatureName].FeatureValue;
                    var previousValue = Convert.ToDouble(baseData[attributeValues[0].Index, attributeToSplit].FeatureValue);
                    foreach (var attributeValue in attributeValues)
                    {
                        var currentValue = attributeValue.Value;
                        var currentRowIndex = attributeValue.Index;
                        var currentClass = baseData[currentRowIndex, dependentFeatureName].FeatureValue;
                        if (!currentClass.Equals(previousClass))
                        {
                            var halfWay = (currentValue + previousValue)/2.0;
                            var splitParams = new BinarySplittingParams<double>(attributeToSplit, halfWay);
                            IList<ISplittedData<bool>> splittedData = _binaryNumericDataSplitter.SplitData(baseData, splitParams);
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
                }
                else
                {
                    var allPossibleValues = baseData.GetColumnVector<TValue>(attributeToSplit).Distinct();
                    foreach (var possibleValue in allPossibleValues)
                    {
                        var splitParams = new BinarySplittingParams<TValue>(attributeToSplit, possibleValue);
                        IList<ISplittedData<bool>> splittedData = _binaryDataSplitter.SplitData(baseData, splitParams);
                        double splitQuality = splitQualityChecker.CalculateSplitQuality(initialEntropy, totalRowsCount,
                            splittedData, dependentFeatureName);
                        if (splitQuality > bestSplitQuality)
                        {
                            bestSplitQuality = splitQuality;
                            bestSplit = new BinarySplittingResult(false, attributeToSplit, splittedData, possibleValue);
                        }
                    }
                }
            }
            return bestSplit;
        }
    }
}
