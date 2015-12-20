using System;
using System.Collections.Generic;
using System.Linq;
using BrainSharper.Abstract.Algorithms.DecisionTrees.DataStructures;
using BrainSharper.Abstract.Algorithms.DecisionTrees.Processors;
using BrainSharper.Abstract.Data;
using BrainSharper.Implementations.Algorithms.DecisionTrees.DataStructures;
using BrainSharper.Implementations.Algorithms.DecisionTrees.DataStructures.BinaryDecisionTrees;
using MathNet.Numerics.LinearAlgebra;
using MathNet.Numerics.LinearAlgebra.Double;

namespace BrainSharper.Implementations.Algorithms.DecisionTrees.Processors
{
    internal struct NumericFeatureData
    {
        public NumericFeatureData(double featureVal, object dependentVal)
        {
            FeatureVal = featureVal;
            DependentVal = dependentVal;
        }

        public double FeatureVal { get; }
        public object DependentVal { get; }
    }

    public class DynamicProgrammingNumericSplitFinder : IBinaryNumericSplitPointSelectorCategoricalOutcome
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
            var uniqueDependentValues = baseData.GetColumnVector(dependentFeatureName).Values.Distinct().ToList();
            var dependentValuesSortedByNumericFeature = OrderColumn(baseData, dependentFeatureName,
                numericFeatureToProcess);
            var dependentValsCounts = new List<Vector<double>>();
            var breakPoints = new List<int>();
            var lastKnowDependentValue = dependentValuesSortedByNumericFeature.First().DependentVal;
            for (var elemIdx = 0; elemIdx < dependentValuesSortedByNumericFeature.Count; elemIdx++)
            {
                var currentElem = dependentValuesSortedByNumericFeature[elemIdx];
                var currentDependentValue = currentElem.DependentVal;
                var indexOfCurrentDependentValue = uniqueDependentValues.IndexOf(currentDependentValue);
                var dependentValsCountAllocation =
                    DenseVector.OfArray(Enumerable.Repeat(0.0, uniqueDependentValues.Count).ToArray());
                if (elemIdx != 0)
                {
                    dependentValsCounts[elemIdx - 1].CopyTo(dependentValsCountAllocation);
                }
                dependentValsCountAllocation[indexOfCurrentDependentValue]++;
                if (!currentDependentValue.Equals(lastKnowDependentValue))
                {
                    lastKnowDependentValue = currentDependentValue;
                    breakPoints.Add(elemIdx - 1);
                }

                dependentValsCounts.Add(dependentValsCountAllocation);
            }

            var bestSplitQualitySoFar = double.NegativeInfinity;
            var bestBreakpointSoFar = -1;
            foreach (var breakpointIdx in breakPoints)
            {
                var dependentValsCountUpToBreakpoint = dependentValsCounts[breakpointIdx];
                var dependentValsCountAboveBreakpoint =
                    dependentValsCounts.Last().Subtract(dependentValsCountUpToBreakpoint);
                var splitEntropy = splitQualityChecker.CalculateSplitQuality(
                    initialEntropy,
                    baseData.RowCount,
                    new List<IList<int>>
                    {
                        VectorToIntArray(dependentValsCountUpToBreakpoint),
                        VectorToIntArray(dependentValsCountAboveBreakpoint)
                    });
                if (splitEntropy > bestSplitQualitySoFar)
                {
                    bestSplitQualitySoFar = splitEntropy;
                    bestBreakpointSoFar = breakpointIdx;
                }
            }

            var splitVal = CalculateSplitPoint(
                dependentValuesSortedByNumericFeature[bestBreakpointSoFar + 1].FeatureVal,
                dependentValuesSortedByNumericFeature[bestBreakpointSoFar].FeatureVal);
            var split = binaryNumericDataSplitter.SplitData(
                baseData,
                new BinarySplittingParams(numericFeatureToProcess, splitVal, dependentFeatureName));
            var splitResult = new BinarySplittingResult(true, numericFeatureToProcess, split, splitVal);
            return new Tuple<ISplittingResult, double>(splitResult, bestSplitQualitySoFar);
        }

        private static List<NumericFeatureData> OrderColumn(IDataFrame baseData, string dependentFeatureName,
            string numericFeatureToProcess)
        {
            return baseData.GetNumericColumnVector(numericFeatureToProcess)
                .Select(
                    (featureVal, rowIndex) =>
                        new NumericFeatureData(featureVal, baseData[rowIndex, dependentFeatureName].FeatureValue))
                .OrderBy(elem => elem.FeatureVal)
                .ToList();
        }

        private double CalculateSplitPoint(double hi, double lo)
        {
            return (hi + lo)/2;
        }

        private int[] VectorToIntArray(Vector<double> vector)
        {
            return vector.Select(elem => (int) elem).ToArray();
        }
    }
}