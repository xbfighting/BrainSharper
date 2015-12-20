using System.Collections.Generic;
using System.Linq;
using BrainSharper.Abstract.Algorithms.RuleInduction.Heuristics;
using BrainSharper.Abstract.Data;

namespace BrainSharper.Implementations.Algorithms.RuleInduction.Heuristics
{
    public class LaplacianSmoothingQualityChecker : IComplexQualityChecker
    {
        private readonly int numberOfCategories;
        private readonly double smoothingWeight;

        public LaplacianSmoothingQualityChecker(int numberOfCategories, double smoothingWeight = 1)
        {
            this.numberOfCategories = numberOfCategories;
            this.smoothingWeight = smoothingWeight;
        }

        public ComplexQualityData CalculateComplexQuality(
            IDataFrame dataFrame,
            string dependentFeatureName,
            IList<int> examplesCoveredByComplex)
        {
            if (!examplesCoveredByComplex.Any())
            {
                return new ComplexQualityData(double.NegativeInfinity);
            }

            var mostCommonDependentFeatureValue =
                dataFrame.GetColumnVector(dependentFeatureName)
                    .Values.Where((val, idx) => examplesCoveredByComplex.Contains(idx))
                    .GroupBy(val => val)
                    .OrderByDescending(grp => grp.Count())
                    .FirstOrDefault();
            var mostCommonDependentFeatureValueCount = mostCommonDependentFeatureValue?.Count();
            if (!mostCommonDependentFeatureValueCount.HasValue)
            {
                return new ComplexQualityData(0.0);
            }
            var nominator = mostCommonDependentFeatureValueCount + (smoothingWeight*(1.0/numberOfCategories));
            var denominator = examplesCoveredByComplex.Count + numberOfCategories;
            var qualityValue = nominator/denominator;
            return new ComplexQualityData(qualityValue.Value, qualityValue == 1.0);
        }
    }
}