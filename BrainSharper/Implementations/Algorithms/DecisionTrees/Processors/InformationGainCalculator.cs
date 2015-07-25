namespace BrainSharper.Implementations.Algorithms.DecisionTrees.Processors
{
    using System.Collections.Generic;

    using Abstract.Algorithms.DecisionTrees.DataStructures;
    using Abstract.Algorithms.DecisionTrees.Processors;
    using Abstract.Data;
    using Abstract.MathUtils.ImpurityMeasures;

    public class InformationGainCalculator<TTestResult, TDecisionType> : ISplitQualityChecker<TTestResult>
    {
        protected readonly IImpurityMeasure<TDecisionType> ImpuryMeasure;
        protected readonly ICategoricalImpurityMeasure<TDecisionType> CategoricalImpuryMeasure;

        public InformationGainCalculator(IImpurityMeasure<TDecisionType> impuryMeasure, ICategoricalImpurityMeasure<TDecisionType> categoricalImpurityMeasure)
        {
            this.ImpuryMeasure = impuryMeasure;
            this.CategoricalImpuryMeasure = categoricalImpurityMeasure;
        }

        public double GetInitialEntropy(IDataFrame baseData, string dependentFeatureName)
        {
            return this.ImpuryMeasure.ImpurityValue(baseData.GetColumnVector<TDecisionType>(dependentFeatureName));
        }

        public virtual double CalculateSplitQuality(IDataFrame baseData, IList<ISplittedData<TTestResult>> splittingResults, string dependentFeatureName)
        {
            double initialEntropy = this.GetInitialEntropy(baseData, dependentFeatureName);
            return this.CalculateSplitQuality(initialEntropy, baseData.RowCount, splittingResults, dependentFeatureName);
        }

        public virtual double CalculateSplitQuality(double initialEntropy, int totalRowsCount, IList<ISplittedData<TTestResult>> splittingResults, string dependentFeatureName)
        {
            var splittedDataWeightedEntopy = this.GetSplittedDataWeightedEntropy(
                splittingResults,
                totalRowsCount,
                dependentFeatureName);
            return initialEntropy - splittedDataWeightedEntopy;
        }

        protected virtual double GetSplittedDataWeightedEntropy(
            IList<ISplittedData<TTestResult>> splittingResults,
            double baseDataRowsCount,
            string dependentFeatureName)
        {
            var splittedDataWeightedEntropy = 0.0;
            foreach (var splittingResult in splittingResults)
            {
                var splittingResultEntropy = this.ImpuryMeasure.ImpurityValue(
                        splittingResult.SplittedDataFrame.GetColumnVector<TDecisionType>(dependentFeatureName));
                var splittedDataCount = splittingResult.SplittedDataFrame.RowCount;
                var weight = splittedDataCount / baseDataRowsCount;
                splittedDataWeightedEntropy += weight * splittingResultEntropy;
            }
            return splittedDataWeightedEntropy;
        }
    }
}
