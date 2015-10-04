namespace BrainSharper.Implementations.Algorithms.RuleInduction.Heuristics
{
    using System.Linq;

    using Abstract.Algorithms.RuleInduction.DataStructures;
    using Abstract.Algorithms.RuleInduction.Heuristics;
    using Abstract.Data;

    public class LaplacianSmoothingQualityChecker<TValue> : IComplexQualityChecker<TValue>
    {
        private readonly int numberOfCategories;
        private readonly double smoothingWeight;

        public LaplacianSmoothingQualityChecker(int numberOfCategories, double smoothingWeight = 1)
        {
            this.numberOfCategories = numberOfCategories;
            this.smoothingWeight = smoothingWeight;
        }

        public double CalculateComplexQuality(
            IDataFrame dataFrame,
            string dependentFeatureName,
            IComplex<TValue> complex,
            IComplexCoveredExamplesInfo<TValue> mnemonics)
        {
            var complexCoveredExamples = mnemonics.ExamplesCoveredByComplexCount(complex);
            if (complexCoveredExamples == 0)
            {
                return double.NegativeInfinity;
            }

            var coveredExampleIndices = mnemonics.ExamplesCoveredByComplex(complex);
            var mostCommonDependentFeatureValueCount =
                dataFrame.GetColumnVector(dependentFeatureName)
                    .Values.Where((val, idx) => coveredExampleIndices.Contains(idx))
                    .GroupBy(val => val)
                    .OrderByDescending(grp => grp.Count())
                    .First()
                    .Count();
            var nominator = mostCommonDependentFeatureValueCount + (smoothingWeight * (1.0 / numberOfCategories));
            var denominator = complexCoveredExamples + numberOfCategories;
            return nominator / denominator;
        }
    }
}
