﻿using System;
using System.Collections.Generic;
using System.Linq;
using BrainSharper.Abstract.Algorithms.RuleInduction;
using BrainSharper.Abstract.Algorithms.RuleInduction.DataStructures;
using BrainSharper.Abstract.Algorithms.RuleInduction.Heuristics;
using BrainSharper.Abstract.Algorithms.RuleInduction.Processors;
using BrainSharper.Abstract.Data;
using BrainSharper.Implementations.Algorithms.RuleInduction.DataStructures;

namespace BrainSharper.Implementations.Algorithms.RuleInduction
{
    public class Cn2Algorithm<TValue> : SequentialCovering<TValue>
    {
        private readonly int beamSize;
        private readonly IComplexesIntersector<TValue> complexIntersector;
        private readonly IComplexQualityChecker complexQualityChecker;
        private readonly IComplexStatisticalImportanceChecker complexStatisticalImportanceChecker;

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
            var seeds = new List<IComplex<TValue>> {new Complex<TValue>()};
            IComplex<TValue> bestComplex = null;
            var bestComplexQuality = double.NegativeInfinity;
            IList<int> examplesCoveredByBestComplex = new int[0];

            var featureDomains = additionalParams.FeatureDomainsToIntersectWith;
            while (seeds.Any())
            {
                var newComplexesSet = complexIntersector.IntersectComplexesWithFeatureDomains(seeds, featureDomains);
                var values = new List<Tuple<IComplex<TValue>, ComplexQualityData>>();
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
                    if (complexQuality.IsBestPossible && examplesCoveredByComplex.Any() &&
                        complexIsStatisticallySignificant)
                    {
                        values.Clear();
                        values.Add(new Tuple<IComplex<TValue>, ComplexQualityData>(complex, complexQuality));
                        bestComplexQuality = complexQuality.QualityValue;
                        examplesCoveredByBestComplex = examplesCoveredByComplex;
                        bestComplex = complex;
                        break;
                    }

                    if (complexIsStatisticallySignificant)
                    {
                        values.Add(new Tuple<IComplex<TValue>, ComplexQualityData>(complex, complexQuality));
                    }

                    if (complexQuality.QualityValue > bestComplexQuality && complexIsStatisticallySignificant)
                    {
                        bestComplexQuality = complexQuality.QualityValue;
                        examplesCoveredByBestComplex = examplesCoveredByComplex;
                        bestComplex = complex;
                    }
                }

                seeds =
                    values.OrderByDescending(kvp => kvp.Item2.QualityValue)
                        .Take(beamSize)
                        .Select(kvp => kvp.Item1)
                        .ToList();
            }

            return new Tuple<IComplex<TValue>, IList<int>>(bestComplex, examplesCoveredByBestComplex);
        }
    }
}