using System;
using System.Linq;
using BrainSharper.Abstract.Algorithms.DecisionTrees.DataStructures;
using BrainSharper.Abstract.Algorithms.DecisionTrees.Processors;
using BrainSharper.Abstract.Data;
using BrainSharper.Implementations.Algorithms.DecisionTrees.DataStructures;
using BrainSharper.Implementations.Algorithms.DecisionTrees.DataStructures.BinaryDecisionTrees;

namespace BrainSharper.Implementations.Algorithms.DecisionTrees.Processors
{
    public class ClassBreakpointsNumericSplitFinder : IBinaryNumericSplitPointSelectorCategoricalOutcome
    {
        public Tuple<ISplittingResult, double> FindBestSplitPoint(
            IDataFrame baseData,
            string dependentFeatureName,
            string numericFeatureToProcess,
            ISplitQualityChecker splitQualityChecker,
            IBinaryNumericDataSplitter binaryNumericDataSplitter,
            double initialEntropy)
        {
            return FindBestSplitPoint(
                baseData,
                dependentFeatureName,
                numericFeatureToProcess,
                splitQualityChecker as ICategoricalSplitQualityChecker,
                binaryNumericDataSplitter,
                initialEntropy);
        }

        public Tuple<ISplittingResult, double> FindBestSplitPoint(
            IDataFrame baseData,
            string dependentFeatureName,
            string numericFeatureToProcess,
            ICategoricalSplitQualityChecker splitQualityChecker,
            IBinaryNumericDataSplitter binaryNumericDataSplitter,
            double initialEntropy)
        {
            ISplittingResult bestSplit = null;
            var bestSplitQuality = double.NegativeInfinity;
            var totalRowsCount = baseData.RowCount;
            var sortedRowData =
                baseData.GetNumericColumnVector(numericFeatureToProcess)
                    .AsParallel()
                    .Select((val, rowIdx) =>
                        new
                        {
                            RowIdx = rowIdx,
                            FeatureValue = val,
                            DependentFeatureValue = baseData[rowIdx, dependentFeatureName].FeatureValue
                        })
                    .OrderBy(elem => elem.FeatureValue).ThenBy(elem => elem.RowIdx)
                    .ToList();
            var previousClass = sortedRowData[0].DependentFeatureValue;
            var previousFeatureVal = sortedRowData[0].FeatureValue;
            foreach (var rowData in sortedRowData)
            {
                var currentClass = rowData.DependentFeatureValue;
                var currentFeatureVal = rowData.FeatureValue;
                if (!currentClass.Equals(previousClass) && !currentFeatureVal.Equals(previousFeatureVal))
                {
                    var halfWay = (previousFeatureVal + currentFeatureVal)/2.0;
                    var splitParams = new BinarySplittingParams(numericFeatureToProcess, halfWay, dependentFeatureName);
                    var splitResult = binaryNumericDataSplitter.SplitData(baseData, splitParams);
                    var quality = splitQualityChecker.CalculateSplitQuality(
                        initialEntropy,
                        totalRowsCount,
                        splitResult,
                        dependentFeatureName);
                    if (quality >= bestSplitQuality)
                    {
                        bestSplitQuality = quality;
                        bestSplit = new BinarySplittingResult(true, numericFeatureToProcess, splitResult, halfWay);
                    }

                    previousClass = currentClass;
                }

                previousFeatureVal = currentFeatureVal;
            }

            return new Tuple<ISplittingResult, double>(bestSplit, bestSplitQuality);
        }
    }
}