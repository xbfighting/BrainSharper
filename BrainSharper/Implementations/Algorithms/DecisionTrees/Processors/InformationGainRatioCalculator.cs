using System.Collections.Generic;
using System.Linq;
using BrainSharper.Abstract.Algorithms.DecisionTrees.DataStructures;
using BrainSharper.Abstract.Data;
using BrainSharper.Abstract.MathUtils.ImpurityMeasures;

namespace BrainSharper.Implementations.Algorithms.DecisionTrees.Processors
{
    public class InformationGainRatioCalculator<T> : InformationGainCalculator<T>
    {
        public InformationGainRatioCalculator(IImpurityMeasure<T> impuryMeasure, ICategoricalImpurityMeasure<T> categoricalImpurityMeasure) 
            : base(impuryMeasure, categoricalImpurityMeasure)
        {
        }

        public override double CalculateSplitQuality(IDataFrame baseData, IList<ISplittedData> splittingResults, string dependentFeatureName)
        {
            var informationGain = base.CalculateSplitQuality(baseData, splittingResults, dependentFeatureName);
            var splitEntropy = GetSplitEntropy(splittingResults, baseData.RowCount);
            return informationGain/splitEntropy;
        }

        public override double CalculateSplitQuality(double initialEntropy, int totalRowsCount, IList<ISplittedData> splittingResults, string dependentFeatureName)
        {
            var informationGain = base.CalculateSplitQuality(initialEntropy, totalRowsCount, splittingResults,
                dependentFeatureName);
            var splittedDataWeightedEntopy = GetSplittedDataWeightedEntropy(
                splittingResults,
                totalRowsCount,
                dependentFeatureName);
            return initialEntropy - splittedDataWeightedEntopy;
        }

        protected virtual double GetSplitEntropy(IList<ISplittedData> splittingResults, double baseDataRowsCount)
        {
            var elementsInGroupsCount = splittingResults.Select(splitResult => splitResult.SplittedDataFrame.RowCount).ToList();
            return CategoricalImpuryMeasure.ImpurityValue(elementsInGroupsCount);
        }
    }
}
