namespace BrainSharper.Implementations.Algorithms.DecisionTrees.Processors
{
    using System;
    using System.Collections.Generic;
    using System.Linq;

    using Abstract.Algorithms.DecisionTrees.DataStructures;
    using Abstract.Algorithms.DecisionTrees.Processors;
    using Abstract.Data;

    using MathNet.Numerics.Statistics;

    public class VarianceBasedSplitQualityChecker : INumericalSplitQualityChecker
    {
        public double GetInitialEntropy(IDataFrame baseData, string dependentFeatureName)
        {
            return baseData.GetNumericColumnVector(dependentFeatureName).Variance();
        }

        public double CalculateSplitQuality(IDataFrame baseData, IList<ISplittedData> splittingResults, string dependentFeatureName)
        {
            var splittedResultsDependentValues =
               splittingResults.Select(
                   res => res.SplittedDataFrame.GetNumericColumnVector(dependentFeatureName) as IList<double>).ToList();
            var initialEntropy = GetInitialEntropy(baseData, dependentFeatureName);
            return CalculateSplitQuality(initialEntropy, baseData.RowCount, splittedResultsDependentValues);
        }

        public double CalculateSplitQuality(
            double initialEntropy,
            int totalRowsCount,
            IList<ISplittedData> splittingResults,
            string dependentFeatureName)
        {
            var splittingResultsDependentValues =
                splittingResults.Select(sr => sr.SplittedDataFrame.GetNumericColumnVector(dependentFeatureName).ToList() as IList<double>)
                    .ToList();
            return CalculateSplitQuality(initialEntropy, totalRowsCount, splittingResultsDependentValues);
        }

        public double CalculateSplitQuality(double initialEntropy, int totalRowsCount, IEnumerable<IList<double>> splittedSubSetsVariances)
        {
            var weightedSplittedVariancesSum = splittedSubSetsVariances.Aggregate(
                0.0,
                (sum, subsetData) => CalculateSubsetVariance(sum, subsetData, totalRowsCount),
                sum => sum);
            return initialEntropy - weightedSplittedVariancesSum;
        }

        private double CalculateSubsetVariance(double sum, IList<double> groupedData, double totalRowsCount)
        {
            var weight = groupedData.Count / totalRowsCount;
            return sum + (weight * groupedData.Variance());
        }
    }
}
