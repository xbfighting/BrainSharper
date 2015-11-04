namespace BrainSharper.Abstract.Algorithms.RuleInduction
{
    using System.Collections.Generic;
    using DataStructures;
    using Infrastructure;

    public interface IRuleInductionParams<TValue> : IModelBuilderParams
    {
        IDictionary<string, ISet<IComplex<TValue>>> FeatureDomainsToIntersectWith { get; }
    }
}
