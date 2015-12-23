namespace BrainSharper.Abstract.Algorithms.AssociationAnalysis.DataStructures
{
    public interface IAssociationRule<TValue> : IAssociationMiningItem
    {
        IFrequentItemsSet<TValue> Antecedent { get; }
        IFrequentItemsSet<TValue> Consequent { get; }
        double Confidence { get; }
    }
}