namespace BrainSharper.Abstract.Algorithms.AssociationAnalysis.DataStructures
{
    using System.Collections.Generic;

    public interface ITransaction<out TValue>
    {
        object TransactionKey { get; }
        IEnumerable<TValue> TransactionItems { get; }
    }
}
