using System.Collections.Generic;
using BrainSharper.Abstract.Algorithms.RuleInduction.DataStructures;
using BrainSharper.Abstract.Data;

namespace BrainSharper.Abstract.Algorithms.RuleInduction.Processors
{
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