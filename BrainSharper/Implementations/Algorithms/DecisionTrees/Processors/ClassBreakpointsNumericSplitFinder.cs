namespace BrainSharper.Implementations.Algorithms.DecisionTrees.Processors
{
    using System;
    using System.Linq;

    using Abstract.Algorithms.DecisionTrees.DataStructures;
    using Abstract.Algorithms.DecisionTrees.Processors;
    using Abstract.Data;
    using DataStructures;
    using DataStructures.BinaryDecisionTrees;

    public class ClassBreakpointsNumericSplitFinder : IBinaryNumericAttributeBestSplitPointSelector
    {
        public Tuple<ISplittingResult, double> FindBestSplitPoint(
            IDataFrame baseData,
            string dependentFeatureName,
            string numericFeatureToProcess,
            ISplitQualityChecker splitQualityChecker,
            IBinaryNumericDataSplitter binaryNumericDataSplitter,
            double initialEntropy)
        {
            ISplittingResult bestSplit = null;
            double bestSplitQuality = double.NegativeInfinity;
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
                    var halfWay = (previousFeatureVal + currentFeatureVal) / 2.0;
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
