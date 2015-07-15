using System.Collections.Generic;
using BrainSharper.Abstract.Algorithms.DecisionTrees.DataStructures;
using BrainSharper.Abstract.Algorithms.DecisionTrees.Processors;
using BrainSharper.Abstract.Data;
using BrainSharper.Abstract.MathUtils.ImpurityMeasures;

namespace BrainSharper.Implementations.Algorithms.DecisionTrees.Processors
{
    public class InformationGainCalculator<T> : ISplitQualityChecker
    {
        protected readonly IImpurityMeasure<T> ImpuryMeasure;

        public InformationGainCalculator(IImpurityMeasure<T> impuryMeasure)
        {
            ImpuryMeasure = impuryMeasure;
        }

        public virtual double CalculateSplitQuality(IDataFrame baseData, IList<ISplittingResult> splittingResults, string dependentFeatureName)
        {
            double initialEntropy = ImpuryMeasure.ImpurityValue(baseData.GetColumnVector<T>(dependentFeatureName));
            var totalRowsCount = baseData.RowCount;
            var splittedDataWeightedEntopy = GetSplittedDataWeightedEntropy(
                splittingResults, 
                totalRowsCount,
                dependentFeatureName);
            return initialEntropy - splittedDataWeightedEntopy;
        }

        protected virtual double GetSplittedDataWeightedEntropy(
            IList<ISplittingResult> splittingResults,
            double baseDataRowsCount,
            string dependentFeatureName)
        {
            var splittedDataWeightedEntropy = 0.0;
            foreach (var splittingResult in splittingResults)
            {
                var splittingResultEntropy =
                    ImpuryMeasure.ImpurityValue(
                        splittingResult.SplittedDataFrame.GetColumnVector<T>(dependentFeatureName));
                var splittedDataCount = splittingResult.SplittedDataFrame.RowCount;
                var weight = splittedDataCount / baseDataRowsCount;
                splittedDataWeightedEntropy += weight * splittingResultEntropy;
            }
            return splittedDataWeightedEntropy;
        }
    }
}
