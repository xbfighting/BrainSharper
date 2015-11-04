namespace BrainSharper.Implementations.Algorithms.RuleInduction.Heuristics
{
    using System.Collections.Generic;
    using System.Linq;

    using Abstract.Algorithms.RuleInduction.Heuristics;
    using Abstract.Data;

    public class LaplacianSmoothingQualityChecker : IComplexQualityChecker
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
            IList<int> examplesCoveredByComplex)
        {
            if (!examplesCoveredByComplex.Any())
            {
                return double.NegativeInfinity;
            }

            var mostCommonDependentFeatureValueCount =
                dataFrame.GetColumnVector(dependentFeatureName)
                    .Values.Where((val, idx) => examplesCoveredByComplex.Contains(idx))
                    .GroupBy(val => val)
                    .OrderByDescending(grp => grp.Count())
                    .First()
                    .Count();
            var nominator = mostCommonDependentFeatureValueCount + (smoothingWeight * (1.0 / numberOfCategories));
            var denominator = examplesCoveredByComplex.Count + numberOfCategories;
            return nominator / denominator;
        }
    }
}
