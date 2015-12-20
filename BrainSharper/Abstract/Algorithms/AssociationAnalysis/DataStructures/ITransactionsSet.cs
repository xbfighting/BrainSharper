using System.Collections.Generic;

namespace BrainSharper.Abstract.Algorithms.AssociationAnalysis.DataStructures
{
    public interface ITransactionsSet<out TValue>
    {
        int TransactionsCount { get; }
        IEnumerable<ITransaction<TValue>> TransactionsList { get; }
    }
}