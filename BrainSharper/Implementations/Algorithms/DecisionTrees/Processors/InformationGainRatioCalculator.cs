using System.Collections.Generic;
using System.Linq;
using BrainSharper.Abstract.Algorithms.DecisionTrees.DataStructures;
using BrainSharper.Abstract.Data;
using BrainSharper.Abstract.MathUtils.ImpurityMeasures;

namespace BrainSharper.Implementations.Algorithms.DecisionTrees.Processors
{
    public class InformationGainRatioCalculator<T> : InformationGainCalculator<T>
    {
        public InformationGainRatioCalculator(IImpurityMeasure<T> impuryMeasure) 
            : base(impuryMeasure)
        {
        }

        public override double CalculateSplitQuality(IDataFrame baseData, IList<ISplittingResult> splittingResults, string dependentFeatureName)
        {
            var informationGain = base.CalculateSplitQuality(baseData, splittingResults, dependentFeatureName);
            var splitEntropy = GetSplitEntropy(splittingResults, baseData.RowCount);
            return informationGain/splitEntropy;
        }

        protected virtual double GetSplitEntropy(IList<ISplittingResult> splittingResults, double baseDataRowsCount)
        {
            var elementsInGroupsCount = splittingResults.Select(splitResult => splitResult.SplittedDataFrame.RowCount).ToList();
            return ImpuryMeasure.ImpurityValue(elementsInGroupsCount);
        }
    }
}
