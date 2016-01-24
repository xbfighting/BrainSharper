using System;
using System.Collections.Generic;
using System.Linq;
using BrainSharper.Abstract.Algorithms.DecisionTrees.DataStructures;
using BrainSharper.Abstract.Algorithms.DecisionTrees.Helpers;
using BrainSharper.Abstract.Data;
using MathNet.Numerics.Distributions;

namespace BrainSharper.Implementations.Algorithms.DecisionTrees.Helpers
{
    public class ChiSquareStatisticalSignificanceChecker : IStatisticalSignificanceChecker
    {
        private readonly double significanceLevel;

        public ChiSquareStatisticalSignificanceChecker(double significanceLevel = 0.05)
        {
            this.significanceLevel = significanceLevel;
        }

        public bool IsSplitStatisticallySignificant(
            IDataFrame initialDataFrame,
            ISplittingResult splittingResults,
            string dependentFeatureName)
        {
            var uniqueDependentValuesCounts = initialDataFrame
                .GetColumnVector(dependentFeatureName)
                .Values
                .GroupBy(elem => elem)
                .ToDictionary(grp => grp.Key, grp => grp.Count()/(double) initialDataFrame.RowCount);
            var chisquareStatisticSum = 0.0;
            if (splittingResults.IsSplitNumeric)
            {
                return true;
            }

            var degreesOfFreedom = (uniqueDependentValuesCounts.Keys.Count - 1)
                                   + (splittingResults.SplittedDataSets.Count - 1);
            foreach (var splittingResult in splittingResults.SplittedDataSets)
            {
                var splitSize = splittingResult.SplittedDataFrame.RowCount;
                var actualDependentFeatureValues =
                    splittingResult.SplittedDataFrame.GetColumnVector(dependentFeatureName)
                        .Values.GroupBy(elem => elem)
                        .ToDictionary(grp => grp.Key, grp => grp.Count());
                foreach (var uniqueDependentValueCount in uniqueDependentValuesCounts)
                {
                    var expectedCount = uniqueDependentValueCount.Value*splitSize;
                    var actualCount = 0;
                    if (actualDependentFeatureValues.ContainsKey(uniqueDependentValueCount.Key))
                    {
                        actualCount = actualDependentFeatureValues[uniqueDependentValueCount.Key];
                    }
                    var actualChisquareValue = Math.Pow(actualCount - expectedCount, 2)/expectedCount;
                    chisquareStatisticSum += actualChisquareValue;
                }
            }

            if (ChiSquared.IsValidParameterSet(degreesOfFreedom))
            {
                var pValue = 1 - ChiSquared.CDF(degreesOfFreedom, chisquareStatisticSum);
                if (pValue < significanceLevel)
                {
                    return true;
                }
            }

            return false;
        }

        public bool IsSplitStatisticallySignificant<TValue>(IList<TValue> initialValuesList, IList<IList<TValue>> splittingResults)
        {
            if (initialValuesList.Count == 0 || splittingResults.Count == 0)
            {
                return false;
            }
            var degreesOfFreedom = (initialValuesList.Distinct().Count() - 1) + (splittingResults.Count - 1);
            if (!ChiSquared.IsValidParameterSet(degreesOfFreedom))
            {
                throw new ArgumentException($"Invalid number of degrees of freedom for ChiSquare distribution: {degreesOfFreedom}!");
            }
            var percentagesOfExpected = initialValuesList
                .GroupBy(val => val)
                .ToDictionary(kvp => kvp.Key, kvp => kvp.Count()/(double) initialValuesList.Count);
            var chiSquareSum = 0.0;
            foreach (var splittingResult in splittingResults)
            {
                var splittingResultCount = splittingResult.Count;
                var actualCounts = splittingResult.GroupBy(val => val).ToDictionary(kvp => kvp.Key, kvp => kvp.Count());
                var expectedActualCounts = actualCounts.Aggregate(0.0, (actualExpectedCount, valueAndCount) =>
                {
                    double expectedPercentage;
                    percentagesOfExpected.TryGetValue(valueAndCount.Key, out expectedPercentage);
                    var expectedValue = expectedPercentage * splittingResultCount;
                    var actualExpectedDiff = Math.Pow(valueAndCount.Value - expectedValue, 2)/expectedValue;
                    return actualExpectedCount + actualExpectedDiff;
                });
                chiSquareSum += expectedActualCounts;
            }
            var statitsicValue = ChiSquared.CDF(degreesOfFreedom, chiSquareSum);
            var pValue = 1 - statitsicValue;
            if (pValue < significanceLevel)
            {
                return true;
            }
            return false;
        }
    }
}