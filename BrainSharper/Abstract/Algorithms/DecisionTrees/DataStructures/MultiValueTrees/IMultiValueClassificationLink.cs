using System.Collections.Generic;

namespace BrainSharper.Abstract.Algorithms.DecisionTrees.DataStructures.MultiValueTrees
{
    public interface IMultiValueClassificationLink<TDecisionValue> : IDecisionTreeLink
    {
        TDecisionValue LinkDecisionValue { get; }
        IDictionary<TDecisionValue, int> ClassesCounts { get; }
    }
}