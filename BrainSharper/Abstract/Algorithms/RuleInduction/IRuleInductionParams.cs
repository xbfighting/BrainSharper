using System.Collections.Generic;
using BrainSharper.Abstract.Algorithms.Infrastructure;
using BrainSharper.Abstract.Algorithms.RuleInduction.DataStructures;

namespace BrainSharper.Abstract.Algorithms.RuleInduction
{
    public interface IRuleInductionParams<TValue> : IModelBuilderParams
    {
        IDictionary<string, ISet<IComplex<TValue>>> FeatureDomainsToIntersectWith { get; }
    }
}