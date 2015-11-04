namespace BrainSharper.Abstract.Algorithms.RuleInduction.Heuristics
{
    using System.Collections.Generic;

    using Data;

    public interface IComplexStatisticalImportanceChecker
    {
        bool IsComplexCoverageStatisticallyImportant(
            IDataFrame dataFrame,
            string dependentFeatureName,
            IList<int> examplesCoveredByComplex);

    }
}
