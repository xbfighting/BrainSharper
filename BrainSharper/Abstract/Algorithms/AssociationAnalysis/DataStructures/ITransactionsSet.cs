namespace BrainSharper.Abstract.Algorithms.AssociationAnalysis.DataStructures
{
    using System.Collections.Generic;

    public interface ITransactionsSet<out TValue>
    {
        int TransactionsCount { get; }
        IEnumerable<ITransaction<TValue>> TransactionsList { get; } 
    }
}
