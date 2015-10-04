namespace BrainSharper.Implementations.Algorithms.RuleInduction
{
    using System;
    using System.Collections.Generic;

    using Abstract.Algorithms.Infrastructure;
    using Abstract.Algorithms.RuleInduction;
    using Abstract.Algorithms.RuleInduction.Heuristics;
    using Abstract.Algorithms.RuleInduction.Processors;
    using Abstract.Data;

    using BrainSharper.Abstract.Algorithms.RuleInduction.DataStructures;
    using BrainSharper.Implementations.Algorithms.RuleInduction.DataStructures;

    public class Cn2Algorithm<TValue> : SequentialCovering<TValue>
    {
        private readonly IComplexStatisticalImportanceChecker<TValue> complexStatisticalImportanceChecker;
        private readonly IComplexQualityChecker<TValue> complexQualityChecker;
        private readonly IComplexesIntersector<TValue> complexIntersector;

        public Cn2Algorithm(
            IComplexStatisticalImportanceChecker<TValue> complexStatisticalImportanceChecker, 
            IComplexQualityChecker<TValue> complexQualityChecker, 
            IComplexesIntersector<TValue> complexIntersector)
        {
            this.complexStatisticalImportanceChecker = complexStatisticalImportanceChecker;
            this.complexQualityChecker = complexQualityChecker;
            this.complexIntersector = complexIntersector;
        }

        protected override Tuple<IComplex<TValue>, IList<int>> FindBestRuleAntecedent(
            IDataFrame dataFrame,
            string dependentFeatureName,
            IList<int> coveredExamples,
            IList<int> remainingExamples,
            IModelBuilderParams additionalParams)
        {
            var seeds = new List<IComplex<TValue>>();
            return null;
        }
    }
}
