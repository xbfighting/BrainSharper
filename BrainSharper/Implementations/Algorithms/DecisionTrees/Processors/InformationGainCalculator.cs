using System.Collections.Generic;
using System.Linq;
using BrainSharper.Abstract.Algorithms.DecisionTrees.DataStructures;
using BrainSharper.Abstract.Algorithms.DecisionTrees.Processors;
using BrainSharper.Abstract.Data;
using BrainSharper.Abstract.MathUtils.ImpurityMeasures;

namespace BrainSharper.Implementations.Algorithms.DecisionTrees.Processors
{
    public class InformationGainCalculator<TDecisionType> : ICategoricalSplitQualityChecker
    {
        protected readonly ICategoricalImpurityMeasure<TDecisionType> CategoricalImpuryMeasure;
        protected readonly IImpurityMeasure<TDecisionType> ImpuryMeasure;

        public InformationGainCalculator(IImpurityMeasure<TDecisionType> impuryMeasure,
            ICategoricalImpurityMeasure<TDecisionType> categoricalImpurityMeasure)
        {
            ImpuryMeasure = impuryMeasure;
            CategoricalImpuryMeasure = categoricalImpurityMeasure;
        }

        public double GetInitialEntropy(IDataFrame baseData, string dependentFeatureName)
            => ImpuryMeasure.ImpurityValue(baseData.GetColumnVector<TDecisionType>(dependentFeatureName));

        public virtual double CalculateSplitQuality(IDataFrame baseData, IList<ISplittedData> splittingResults,
            string dependentFeatureName)
        {
            var initialEntropy = GetInitialEntropy(baseData, dependentFeatureName);
            return CalculateSplitQuality(initialEntropy, baseData.RowCount, splittingResults, dependentFeatureName);
        }

        public virtual double CalculateSplitQuality(double initialEntropy, int totalRowsCount,
            IList<ISplittedData> splittingResults, string dependentFeatureName)
        {
            var splittedDataWeightedEntopy = GetSplittedDataWeightedEntropy(
                splittingResults,
                totalRowsCount,
                dependentFeatureName);
            return initialEntropy - splittedDataWeightedEntopy;
        }

        public double CalculateSplitQuality(double initialEntropy, int totalRowsCount,
            IList<IList<int>> elementsInGroupsCounts)
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
                var weight = groupTotalCount/baseTotalRowsCount;
                splittedDataWeightedEntropy += weight*groupImpurity;
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
                var weight = splittedDataCount/baseDataRowsCount;
                splittedDataWeightedEntropy += weight*splittingResultEntropy;
            }
            return splittedDataWeightedEntropy;
        }
    }
}