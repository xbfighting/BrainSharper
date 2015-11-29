namespace BrainSharper.Abstract.Algorithms.AssociationAnalysis
{
    using DataStructures;

    public interface IFrequentItemsFinder<TValue>
    {
        IFrequentItemsSet<IFrequentItemsSet<TValue>> FindFrequentItems(ITransactionsSet<TValue> transactions, IAssociationMiningParams associationMiningParams);
    }
}
