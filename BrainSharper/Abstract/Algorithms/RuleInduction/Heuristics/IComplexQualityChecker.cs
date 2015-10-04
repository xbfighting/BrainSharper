namespace BrainSharper.Abstract.Algorithms.RuleInduction.Heuristics
{
    using BrainSharper.Abstract.Algorithms.RuleInduction.DataStructures;
    using BrainSharper.Abstract.Data;

    public interface IComplexQualityChecker<TValue>
    {
        double CalculateComplexQuality(
            IDataFrame dataFrame, 
            string dependentFeatureName,
            IComplex<TValue> complex, 
            IComplexCoveredExamplesInfo<TValue> mnemonics);
    }
}
