namespace BrainSharper.Abstract.Algorithms.AssociationAnalysis.DataStructures
{
    public interface IAssociationRule<TValue> : IAssociationMiningItem
    {
        IFrequentItemsSet<TValue> Antecedent { get; }
        IFrequentItemsSet<TValue> Consequent { get; }
        bool IsAntecedentNegated { get; }
        bool IsConsequentNegated { get; }
        double Confidence { get; }
        double Lift { get; }
    }
}