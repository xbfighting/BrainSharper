namespace BrainSharper.Abstract.Algorithms.RuleInduction.Processors
{
    using System.Collections.Generic;
    using Data;
    using DataStructures;

    public interface IComplexesIntersector<TValue>
    {
        //TODO: AAA !!! think about using custom feature domain class
        IList<IComplex<TValue>> IntersectComplexesWithFeatureDomains(
            IList<IComplex<TValue>> complexesToIntersect,
            IDictionary<string, ISet<IComplex<TValue>>> featureDomains);

        IDictionary<string, ISet<IComplex<TValue>>> PrepareFeatureDomains(
            IDataFrame dataFrame,
            string dependentFeatureName);
    }
}
