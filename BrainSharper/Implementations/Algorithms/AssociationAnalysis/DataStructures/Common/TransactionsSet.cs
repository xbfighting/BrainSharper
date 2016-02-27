using System.Collections.Generic;
using System.Linq;
using BrainSharper.Abstract.Algorithms.AssociationAnalysis.DataStructures;

namespace BrainSharper.Implementations.Algorithms.AssociationAnalysis.DataStructures.Common
{
    public class TransactionsSet<TValue> : ITransactionsSet<TValue>
    {
        public TransactionsSet(IEnumerable<ITransaction<TValue>> transactionsList)
        {
            TransactionsList = transactionsList.ToList();
        }

        public int TransactionsCount => TransactionsList.Count();
        public IEnumerable<ITransaction<TValue>> TransactionsList { get; }
    }
}