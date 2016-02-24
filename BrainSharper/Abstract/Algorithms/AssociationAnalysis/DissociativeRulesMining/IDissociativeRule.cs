using BrainSharper.Abstract.Algorithms.AssociationAnalysis.DataStructures;

namespace BrainSharper.Abstract.Algorithms.AssociationAnalysis.DissociativeRulesMining
{
    public interface IDissociativeRule<TValue> : IAssociationRule<TValue>
    {
        double Join { get; }
    }
}