using System.Collections.Generic;
using BrainSharper.Abstract.Algorithms.RuleInduction.Heuristics;
using BrainSharper.Abstract.Data;
using BrainSharper.Abstract.MathUtils.ImpurityMeasures;

namespace BrainSharper.Implementations.Algorithms.RuleInduction.Heuristics
{
    public class ImpurityBasedComplexQualityChecker<TValue> : IComplexQualityChecker
    {
        private readonly IImpurityMeasure<TValue> impurityMeasure;

        public ImpurityBasedComplexQualityChecker(IImpurityMeasure<TValue> impurity)
        {
            impurityMeasure = impurity;
        }

        public ComplexQualityData CalculateComplexQuality(
            IDataFrame dataFrame,
            string dependentFeatureName,
            IList<int> examplesCoveredByComplex)
        {
            var dependentValuesCoveredByComplex =
                dataFrame.GetSubsetByRows(examplesCoveredByComplex, true)
                    .GetColumnVector<TValue>(dependentFeatureName)
                    .Values;
            var impurityValue = -impurityMeasure.ImpurityValue(dependentValuesCoveredByComplex);
            var isMaximal = impurityValue == 0.0;
            return new ComplexQualityData(-1*impurityValue, isMaximal);
        }
    }
}