using System.Collections.Generic;

namespace BrainSharper.Abstract.Algorithms.DecisionTrees.DataStructures.BinaryTrees
{
    public interface IBinaryDecisionTreeClassificationLink<TDecisionValue> : IBinaryDecisionTreeLink
    {
        IDictionary<TDecisionValue, int> ClassesCounts { get; }
    }
}
