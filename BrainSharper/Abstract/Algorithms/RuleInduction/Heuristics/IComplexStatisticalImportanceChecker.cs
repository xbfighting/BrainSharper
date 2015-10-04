namespace BrainSharper.Abstract.Algorithms.RuleInduction.Heuristics
{
    using Data;
    using DataStructures;

    public interface IComplexStatisticalImportanceChecker<TValue>
    {
        bool IsComplexCoverageStatisticallyImportant(
            IDataFrame dataFrame,
            string dependentFeatureName,
            IComplex<TValue> complex,
            IComplexCoveredExamplesInfo<TValue> mnemonics);
    }
}
