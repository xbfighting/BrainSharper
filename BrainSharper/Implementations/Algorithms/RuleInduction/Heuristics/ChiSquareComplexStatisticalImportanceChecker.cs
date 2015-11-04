namespace BrainSharper.Implementations.Algorithms.RuleInduction.Heuristics
{
    using System;
    using System.Collections.Generic;
    using System.Linq;

    using Abstract.Algorithms.RuleInduction.Heuristics;
    using Abstract.Data;

    public class ChiSquareComplexStatisticalImportanceChecker<TValue> : IComplexStatisticalImportanceChecker
    {
        private readonly double significanceLevel;

        public ChiSquareComplexStatisticalImportanceChecker(double significanceLevel = 0.05)
        {
            this.significanceLevel = significanceLevel;
        }

        public bool IsComplexCoverageStatisticallyImportant(
            IDataFrame dataFrame,
            string dependentFeatureName,
            IList<int> examplesCoveredByComplex)
        {
            var expectedDependentFeatureValues =
                dataFrame.GetColumnVector<TValue>(dependentFeatureName)
                    .Values.GroupBy(val => val)
                    .ToDictionary(grp => grp.Key, grp => examplesCoveredByComplex.Count * (grp.Count() / (float)dataFrame.RowCount));
            var actualDependentFeatureValues =
                dataFrame.GetSubsetByRows(examplesCoveredByComplex, true)
                .GetColumnVector<TValue>(dependentFeatureName).Values
                .GroupBy(val => val)
                .ToDictionary(grp => grp.Key, grp => grp.Count());
            var chiSquareSum =
                expectedDependentFeatureValues.Sum(
                    kvp => CalculateChiSquareValue(kvp.Value, this.GetActualCount(kvp.Key, actualDependentFeatureValues)));
            var degreesOfFreedom = expectedDependentFeatureValues.Keys.Count - 1;
            if (MathNet.Numerics.Distributions.ChiSquared.IsValidParameterSet(degreesOfFreedom))
            {
                var pValue = 1 - MathNet.Numerics.Distributions.ChiSquared.CDF(degreesOfFreedom, chiSquareSum);
                return pValue < significanceLevel;
            }

            return false;
        }

        protected double CalculateChiSquareValue(double expected, double actual)
        {
            return Math.Pow((actual - expected), 2) / expected;
        }

        protected double GetActualCount(TValue value, IDictionary<TValue, int> valuesCounts)
        {
            int count;
            if (valuesCounts.TryGetValue(value, out count))
            {
                return count;
            }
            return 0;
        }
    }
}
