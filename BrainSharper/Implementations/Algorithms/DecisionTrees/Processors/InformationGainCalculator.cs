namespace BrainSharper.Implementations.Algorithms.DecisionTrees.Processors
{
    using System.Collections.Generic;
    using System.Linq;

    using Abstract.Algorithms.DecisionTrees.DataStructures;
    using Abstract.Algorithms.DecisionTrees.Processors;
    using Abstract.Data;
    using Abstract.MathUtils.ImpurityMeasures;

    public class InformationGainCalculator<TDecisionType> : ICategoricalSplitQualityChecker
    {
        protected readonly IImpurityMeasure<TDecisionType> ImpuryMeasure;
        protected readonly ICategoricalImpurityMeasure<TDecisionType> CategoricalImpuryMeasure;

        public InformationGainCalculator(IImpurityMeasure<TDecisionType> impuryMeasure, ICategoricalImpurityMeasure<TDecisionType> categoricalImpurityMeasure)
        {
            ImpuryMeasure = impuryMeasure;
            CategoricalImpuryMeasure = categoricalImpurityMeasure;
        }

        public double GetInitialEntropy(IDataFrame baseData, string dependentFeatureName) => this.ImpuryMeasure.ImpurityValue(baseData.GetColumnVector<TDecisionType>(dependentFeatureName));

        public virtual double CalculateSplitQuality(IDataFrame baseData, IList<ISplittedData> splittingResults, string dependentFeatureName)
        {
            double initialEntropy = GetInitialEntropy(baseData, dependentFeatureName);
            return CalculateSplitQuality(initialEntropy, baseData.RowCount, splittingResults, dependentFeatureName);
        }

        public virtual double CalculateSplitQuality(double initialEntropy, int totalRowsCount, IList<ISplittedData> splittingResults, string dependentFeatureName)
        {
            var splittedDataWeightedEntopy = GetSplittedDataWeightedEntropy(
                splittingResults,
                totalRowsCount,
                dependentFeatureName);
            return initialEntropy - splittedDataWeightedEntopy;
        }

        public double CalculateSplitQuality(double initialEntropy, int totalRowsCount, IList<IList<int>> elementsInGroupsCounts)
        {
            var splittedDataWeightedEntropy = GetSplittedDataWeightedEntropyFromCounts(
                elementsInGroupsCounts,
                totalRowsCount);
            return initialEntropy - splittedDataWeightedEntropy;
        }

        protected virtual double GetSplittedDataWeightedEntropyFromCounts(
            IList<IList<int>> elementsInGroupsCount,
            double baseTotalRowsCount)
        {
            var splittedDataWeightedEntropy = 0.0;
            foreach (var group in elementsInGroupsCount)
            {
                var groupImpurity = CategoricalImpuryMeasure.ImpurityValue(group);
                var groupTotalCount = group.Sum();
                var weight = groupTotalCount / baseTotalRowsCount;
                splittedDataWeightedEntropy += weight * groupImpurity;
            }
            return splittedDataWeightedEntropy;
        }

        protected virtual double GetSplittedDataWeightedEntropy(
            IList<ISplittedData> splittingResults,
            double baseDataRowsCount,
            string dependentFeatureName)
        {
            var splittedDataWeightedEntropy = 0.0;
            foreach (var splittingResult in splittingResults)
            {
                var splittingResultEntropy = ImpuryMeasure.ImpurityValue(
                        splittingResult.SplittedDataFrame.GetColumnVector<TDecisionType>(dependentFeatureName));
                var splittedDataCount = splittingResult.SplittedDataFrame.RowCount;
                var weight = splittedDataCount / baseDataRowsCount;
                splittedDataWeightedEntropy += weight * splittingResultEntropy;
            }
            return splittedDataWeightedEntropy;
        }
    }
}
