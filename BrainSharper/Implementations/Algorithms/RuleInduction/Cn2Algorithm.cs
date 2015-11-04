namespace BrainSharper.Implementations.Algorithms.RuleInduction
{
    using System;
    using System.Collections.Generic;
    using System.Linq;

    using Abstract.Algorithms.Infrastructure;
    using Abstract.Algorithms.RuleInduction.DataStructures;
    using Abstract.Algorithms.RuleInduction.Heuristics;
    using Abstract.Algorithms.RuleInduction.Processors;
    using Abstract.Data;

    using BrainSharper.Abstract.Algorithms.RuleInduction;

    using DataStructures;

    public class Cn2Algorithm<TValue> : SequentialCovering<TValue>
    {
        private readonly IComplexStatisticalImportanceChecker complexStatisticalImportanceChecker;
        private readonly IComplexQualityChecker complexQualityChecker;
        private readonly IComplexesIntersector<TValue> complexIntersector;
        private readonly int beamSize;

        public Cn2Algorithm(
            IComplexStatisticalImportanceChecker complexStatisticalImportanceChecker, 
            IComplexQualityChecker complexQualityChecker, 
            IComplexesIntersector<TValue> complexIntersector,
            int beamSize)
        {
            this.complexStatisticalImportanceChecker = complexStatisticalImportanceChecker;
            this.complexQualityChecker = complexQualityChecker;
            this.complexIntersector = complexIntersector;
            this.beamSize = beamSize;
        }

        protected override Tuple<IComplex<TValue>, IList<int>> FindBestRuleAntecedent(
            IDataFrame dataFrame,
            string dependentFeatureName,
            IList<int> coveredExamples,
            IList<int> remainingExamples,
            IRuleInductionParams<TValue> additionalParams)
        {
            var seeds = new List<IComplex<TValue>> { new Complex<TValue>() };
            IComplex<TValue> bestComplex = null;
            double bestComplexQuality = double.NegativeInfinity;
            IList<int> examplesCoveredByBestComplex = new int[0];

            var featureDomains = additionalParams.FeatureDomainsToIntersectWith;
            while (seeds.Any())
            {
                var newComplexesSet = complexIntersector.IntersectComplexesWithFeatureDomains(seeds, featureDomains);
                var values = new List<Tuple<IComplex<TValue>, double>>();
                foreach (var complex in newComplexesSet)
                {
                    var examplesCoveredByComplex = new List<int>();
                    foreach (var rowIdx in remainingExamples)
                    {
                        var row = dataFrame.GetRowVector<TValue>(rowIdx, true);
                        if (complex.Covers(row))
                        {
                            examplesCoveredByComplex.Add(rowIdx);
                        }
                    }

                    var complexQuality = complexQualityChecker.CalculateComplexQuality(
                        dataFrame,
                        dependentFeatureName,
                        examplesCoveredByComplex);
                    var complexIsStatisticallySignificant =
                        complexStatisticalImportanceChecker.IsComplexCoverageStatisticallyImportant(
                            dataFrame,
                            dependentFeatureName,
                            examplesCoveredByComplex);
                    if (complexIsStatisticallySignificant)
                    {
                        values.Add(new Tuple<IComplex<TValue>, double>(complex, complexQuality));
                    }
                    if (complexQuality > bestComplexQuality && complexIsStatisticallySignificant)
                    {
                        bestComplexQuality = complexQuality;
                        examplesCoveredByBestComplex = examplesCoveredByComplex;
                        bestComplex = complex;
                    }
                }
                seeds = values.OrderByDescending(kvp => kvp.Item2).Take(beamSize).Select(kvp => kvp.Item1).ToList();
            }

            return new Tuple<IComplex<TValue>, IList<int>>(bestComplex, examplesCoveredByBestComplex);
        }
    }
}
