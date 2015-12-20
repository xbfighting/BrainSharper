using System.Collections.Generic;
using BrainSharper.Abstract.Algorithms.RuleInduction;
using BrainSharper.Abstract.Algorithms.RuleInduction.DataStructures;

namespace BrainSharper.Implementations.Algorithms.RuleInduction
{
    public class RuleInductionParams<TValue> : IRuleInductionParams<TValue>
    {
        public RuleInductionParams(IDictionary<string, ISet<IComplex<TValue>>> featureDomainsToIntersectWith)
        {
            FeatureDomainsToIntersectWith = featureDomainsToIntersectWith;
        }

        public IDictionary<string, ISet<IComplex<TValue>>> FeatureDomainsToIntersectWith { get; }
    }
}