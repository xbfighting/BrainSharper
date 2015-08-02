namespace BrainSharper.Abstract.Algorithms.DecisionTrees.DataStructures.MultiValueTrees
{
    using System.Collections.Generic;

    public interface IMultiValueClassificationLink<TDecisionValue> : IDecisionTreeLink
    {
        TDecisionValue LinkDecisionValue { get; }
        IDictionary<TDecisionValue, int> ClassesCounts { get; }
    }
}
