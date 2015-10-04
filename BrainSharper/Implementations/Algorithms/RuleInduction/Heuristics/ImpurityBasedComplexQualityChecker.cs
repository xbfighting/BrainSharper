namespace BrainSharper.Implementations.Algorithms.RuleInduction.Heuristics
{
    using BrainSharper.Abstract.Algorithms.RuleInduction.DataStructures;
    using BrainSharper.Abstract.Algorithms.RuleInduction.Heuristics;
    using BrainSharper.Abstract.Data;
    using BrainSharper.Abstract.MathUtils.ImpurityMeasures;

    public class ImpurityBasedComplexQualityChecker<TValue> : IComplexQualityChecker<TValue>
    {
        private readonly IImpurityMeasure<TValue> impurityMeasure;

        public ImpurityBasedComplexQualityChecker(IImpurityMeasure<TValue> impurity)
        {
            this.impurityMeasure = impurity;
        }

        public double CalculateComplexQuality(
            IDataFrame dataFrame,
            string dependentFeatureName,
            IComplex<TValue> complex,
            IComplexCoveredExamplesInfo<TValue> mnemonics)
        {
            var examplesCoveredByComplex = mnemonics.ExamplesCoveredByComplex(complex);
            var dependentValuesCoveredByComplex =
                dataFrame.GetSubsetByRows(examplesCoveredByComplex).GetColumnVector<TValue>(dependentFeatureName).Values;
            return -1 * this.impurityMeasure.ImpurityValue(dependentValuesCoveredByComplex);
        }
    }
}
