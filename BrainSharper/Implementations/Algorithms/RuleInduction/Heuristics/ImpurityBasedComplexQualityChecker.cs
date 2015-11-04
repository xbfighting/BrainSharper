namespace BrainSharper.Implementations.Algorithms.RuleInduction.Heuristics
{
    using System.Collections.Generic;

    using Abstract.Algorithms.RuleInduction.Heuristics;
    using Abstract.Data;
    using Abstract.MathUtils.ImpurityMeasures;

    public class ImpurityBasedComplexQualityChecker<TValue> : IComplexQualityChecker
    {
        private readonly IImpurityMeasure<TValue> impurityMeasure;

        public ImpurityBasedComplexQualityChecker(IImpurityMeasure<TValue> impurity)
        {
            this.impurityMeasure = impurity;
        }

        public double CalculateComplexQuality(
            IDataFrame dataFrame,
            string dependentFeatureName,
            IList<int> examplesCoveredByComplex)
        {
            var dependentValuesCoveredByComplex =
                dataFrame.GetSubsetByRows(examplesCoveredByComplex, true).GetColumnVector<TValue>(dependentFeatureName).Values;
            return -1 * this.impurityMeasure.ImpurityValue(dependentValuesCoveredByComplex);
        }
    }
}
