namespace BrainSharper.Implementations.Algorithms.RuleInduction
{
    using System.Collections.Generic;
    using Abstract.Algorithms.RuleInduction;
    using Abstract.Algorithms.RuleInduction.DataStructures;

    public class RuleInductionParams<TValue> : IRuleInductionParams<TValue>
    {
        public RuleInductionParams(IDictionary<string, ISet<IComplex<TValue>>> featureDomainsToIntersectWith)
        {
            FeatureDomainsToIntersectWith = featureDomainsToIntersectWith;
        }

        public IDictionary<string, ISet<IComplex<TValue>>> FeatureDomainsToIntersectWith { get; }
    }
}
