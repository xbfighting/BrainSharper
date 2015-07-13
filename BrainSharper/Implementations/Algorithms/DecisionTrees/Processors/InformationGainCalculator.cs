using System.Collections.Generic;
using BrainSharper.Abstract.Algorithms.DecisionTrees.DataStructures;
using BrainSharper.Abstract.Algorithms.DecisionTrees.Processors;
using BrainSharper.Abstract.Data;
using BrainSharper.Abstract.MathUtils.ImpurityMeasures;

namespace BrainSharper.Implementations.Algorithms.DecisionTrees.Processors
{
    public class InformationGainCalculator<T> : ISplitQualityChecker
    {
        private readonly IImpurityMeasure<T> _impuryMeasure;

        public InformationGainCalculator(IImpurityMeasure<T> impuryMeasure)
        {
            _impuryMeasure = impuryMeasure;
        }

        public double CalculateSplitQuality(IDataFrame baseData, IList<ISplittingResult> splittingResults, string dependentFeatureName)
        {
            double initialEntropy = _impuryMeasure.ImpurityValue(baseData.GetColumnVector<T>(dependentFeatureName));
            var splittedDataWeightedEntropy = 0.0;
            var totalRowsCount = baseData.RowCount;
            foreach (var splittingResult in splittingResults)
            {
                var splittingResultEntropy =
                    _impuryMeasure.ImpurityValue(
                        splittingResult.SplittedDataFrame.GetColumnVector<T>(dependentFeatureName));
                var splittedDataCount = splittingResult.SplittedDataFrame.RowCount;
                var weight = splittedDataCount/(double) totalRowsCount;
                splittedDataWeightedEntropy += weight * splittingResultEntropy;
            }
            return initialEntropy - splittedDataWeightedEntropy;
        }
    }
}
