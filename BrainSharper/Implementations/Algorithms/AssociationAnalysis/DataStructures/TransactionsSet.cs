namespace BrainSharper.Implementations.Algorithms.AssociationAnalysis.DataStructures
{
    using System.Collections.Generic;
    using System.Linq;

    using Abstract.Algorithms.AssociationAnalysis.DataStructures;

    public class TransactionsSet<TValue> : ITransactionsSet<TValue>
    {
        public TransactionsSet(IEnumerable<ITransaction<TValue>> transactionsList)
        {
            this.TransactionsList = transactionsList.ToList();
        }

        public int TransactionsCount => TransactionsList.Count();

        public IEnumerable<ITransaction<TValue>> TransactionsList { get; }
    }
}
