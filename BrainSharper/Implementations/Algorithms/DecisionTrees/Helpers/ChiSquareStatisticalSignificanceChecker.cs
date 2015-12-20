using System;
using System.Linq;
using BrainSharper.Abstract.Algorithms.DecisionTrees.DataStructures;
using BrainSharper.Abstract.Algorithms.DecisionTrees.Helpers;
using BrainSharper.Abstract.Data;
using MathNet.Numerics.Distributions;

namespace BrainSharper.Implementations.Algorithms.DecisionTrees.Helpers
{
    //TODO: make it common with other chisquare mathods
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
                var statisticValue = 1 - ChiSquared.CDF(degreesOfFreedom, chisquareStatisticSum);
                if (statisticValue < significanceLevel)
                {
                    return true;
                }
            }

            return false;
        }
    }
}