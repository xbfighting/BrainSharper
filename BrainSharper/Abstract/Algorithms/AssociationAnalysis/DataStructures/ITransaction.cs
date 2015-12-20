using System.Collections.Generic;

namespace BrainSharper.Abstract.Algorithms.AssociationAnalysis.DataStructures
{
    public interface ITransaction<out TValue>
    {
        object TransactionKey { get; }
        IEnumerable<TValue> TransactionItems { get; }
    }
}