namespace BrainSharper.Abstract.Algorithms.RuleInduction.Heuristics
{
    using System.Collections.Generic;

    using BrainSharper.Abstract.Data;

    public interface IComplexQualityChecker
    {
        //TODO: !!! AAA Add support for passing covered examples count directly. Remove unneccessary dictionary with mnemonics

        double CalculateComplexQuality(
            IDataFrame dataFrame, 
            string dependentFeatureName,
            IList<int> examplesCoveredByComplex);
    }
}
