namespace BrainSharper.Implementations.Algorithms.DecisionTrees.Processors
{
    using System.Collections.Generic;
    using System.Linq;

    using Abstract.Algorithms.DecisionTrees.DataStructures;
    using Abstract.Data;
    using Abstract.MathUtils.ImpurityMeasures;

    public class InformationGainRatioCalculator<TDecisionType> : InformationGainCalculator<TDecisionType>
    {
        public InformationGainRatioCalculator(IImpurityMeasure<TDecisionType> impuryMeasure, ICategoricalImpurityMeasure<TDecisionType> categoricalImpurityMeasure) 
            : base(impuryMeasure, categoricalImpurityMeasure)
        {
        }

        public override double CalculateSplitQuality(IDataFrame baseData, IList<ISplittedData> splittingResults, string dependentFeatureName)
        {
            var informationGain = base.CalculateSplitQuality(baseData, splittingResults, dependentFeatureName);
            var splitEntropy = this.GetSplitEntropy(splittingResults, baseData.RowCount);
            return informationGain / splitEntropy;
        }

        public override double CalculateSplitQuality(
            double initialEntropy, 
            int totalRowsCount, 
            IList<ISplittedData> splittingResults, 
            string dependentFeatureName)
        {
            var splittedDataWeightedEntopy = this.GetSplittedDataWeightedEntropy(
                splittingResults,
                totalRowsCount,
                dependentFeatureName);
            return initialEntropy - splittedDataWeightedEntopy;
        }

        protected virtual double GetSplitEntropy(IList<ISplittedData> splittingResults, double baseDataRowsCount)
        {
            var elementsInGroupsCount = splittingResults.Select(splitResult => splitResult.SplittedDataFrame.RowCount).ToList();
            return this.CategoricalImpuryMeasure.ImpurityValue(elementsInGroupsCount);
        }
    }
}
