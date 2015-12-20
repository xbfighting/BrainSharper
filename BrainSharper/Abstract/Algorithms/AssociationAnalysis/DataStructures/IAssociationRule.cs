namespace BrainSharper.Abstract.Algorithms.AssociationAnalysis.DataStructures
{
    public interface IAssociationRule<TValue>
    {
        IFrequentItemsSet<TValue> Antecedent { get; }
        IFrequentItemsSet<TValue> Consequent { get; }
    }
}