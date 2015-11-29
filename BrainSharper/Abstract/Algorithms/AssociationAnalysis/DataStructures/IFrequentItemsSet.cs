namespace BrainSharper.Abstract.Algorithms.AssociationAnalysis.DataStructures
{
    using System.Collections.Generic;

    public interface IFrequentItemsSet<TValue>
    {
        ISet<TValue> ItemsSet { get; }
        IList<TValue> OrderedItems { get; }
        double Support { get; }
        double RelativeSuppot { get; }
        ISet<object> TransactionIds { get; }
    }
}
