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

        public ComplexQualityData CalculateComplexQuality(
            IDataFrame dataFrame,
            string dependentFeatureName,
            IList<int> examplesCoveredByComplex)
        {
            var dependentValuesCoveredByComplex =
                dataFrame.GetSubsetByRows(examplesCoveredByComplex, true).GetColumnVector<TValue>(dependentFeatureName).Values;
            var impurityValue = -impurityMeasure.ImpurityValue(dependentValuesCoveredByComplex);
            var isMaximal = impurityValue == 0.0;
            return new ComplexQualityData(-1 * impurityValue, isMaximal);
        }
    }
}
