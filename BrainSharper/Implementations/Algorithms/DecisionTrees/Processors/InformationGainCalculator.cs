using System.Collections.Generic;
using BrainSharper.Abstract.Algorithms.DecisionTrees.DataStructures;
using BrainSharper.Abstract.Algorithms.DecisionTrees.Processors;
using BrainSharper.Abstract.Data;
using BrainSharper.Abstract.MathUtils.ImpurityMeasures;

namespace BrainSharper.Implementations.Algorithms.DecisionTrees.Processors
{
    public class InformationGainCalculator<TTestResult, TDecisionType> : ISplitQualityChecker<TTestResult>
    {
        protected readonly IImpurityMeasure<TDecisionType> ImpuryMeasure;
        protected readonly ICategoricalImpurityMeasure<TDecisionType> CategoricalImpuryMeasure;


        public InformationGainCalculator(IImpurityMeasure<TDecisionType> impuryMeasure, ICategoricalImpurityMeasure<TDecisionType> categoricalImpurityMeasure)
        {
            ImpuryMeasure = impuryMeasure;
            CategoricalImpuryMeasure = categoricalImpurityMeasure;
        }

        public double GetInitialEntropy(IDataFrame baseData, string dependentFeatureName)
        {
            return ImpuryMeasure.ImpurityValue(baseData.GetColumnVector<TDecisionType>(dependentFeatureName));
        }

        public virtual double CalculateSplitQuality(IDataFrame baseData, IList<ISplittedData<TTestResult>> splittingResults, string dependentFeatureName)
        {
            double initialEntropy = GetInitialEntropy(baseData, dependentFeatureName);
            return CalculateSplitQuality(initialEntropy, baseData.RowCount, splittingResults, dependentFeatureName);
        }

        public virtual double CalculateSplitQuality(double initialEntropy, int totalRowsCount, IList<ISplittedData<TTestResult>> splittingResults, string dependentFeatureName)
        {
            var splittedDataWeightedEntopy = GetSplittedDataWeightedEntropy(
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
                var splittingResultEntropy =
                    ImpuryMeasure.ImpurityValue(
                        splittingResult.SplittedDataFrame.GetColumnVector<TDecisionType>(dependentFeatureName));
                var splittedDataCount = splittingResult.SplittedDataFrame.RowCount;
                var weight = splittedDataCount / baseDataRowsCount;
                splittedDataWeightedEntropy += weight * splittingResultEntropy;
            }
            return splittedDataWeightedEntropy;
        }
    }
}
