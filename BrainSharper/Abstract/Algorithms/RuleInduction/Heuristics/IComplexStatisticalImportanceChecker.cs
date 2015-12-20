using System.Collections.Generic;
using BrainSharper.Abstract.Data;

namespace BrainSharper.Abstract.Algorithms.RuleInduction.Heuristics
{
    public interface IComplexStatisticalImportanceChecker
    {
        bool IsComplexCoverageStatisticallyImportant(
            IDataFrame dataFrame,
            string dependentFeatureName,
            IList<int> examplesCoveredByComplex);
    }
}