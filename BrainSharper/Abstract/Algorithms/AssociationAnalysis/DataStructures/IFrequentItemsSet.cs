using System.Collections.Generic;

namespace BrainSharper.Abstract.Algorithms.AssociationAnalysis.DataStructures
{
    public interface IFrequentItemsSet<TValue> : IAssociationMiningItem
    {
        ISet<TValue> ItemsSet { get; }
        IList<TValue> OrderedItems { get; }
        
        ISet<object> TransactionIds { get; }
        bool KFirstElementsEqual(IFrequentItemsSet<TValue> other, int k);
        bool ItemsOnlyEqual(IFrequentItemsSet<TValue> other);
    }
}