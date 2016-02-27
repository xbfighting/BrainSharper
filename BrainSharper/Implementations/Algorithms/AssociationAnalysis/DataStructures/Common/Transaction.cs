using System.Collections.Generic;
using System.Linq;
using BrainSharper.Abstract.Algorithms.AssociationAnalysis.DataStructures;

namespace BrainSharper.Implementations.Algorithms.AssociationAnalysis.DataStructures.Common
{
    public struct Transaction<TValue> : ITransaction<TValue>
    {
        public Transaction(object transactionKey, IEnumerable<TValue> transactionItems)
        {
            TransactionKey = transactionKey;
            TransactionItems = transactionItems;
        }

        public Transaction(object transactionKey, params TValue[] transactionItems)
        {
            TransactionKey = transactionKey;
            TransactionItems = transactionItems;
        }

        public object TransactionKey { get; }
        public IEnumerable<TValue> TransactionItems { get; }

        public bool Equals(Transaction<TValue> other)
        {
            return
                Equals(TransactionKey, other.TransactionKey) &&
                TransactionItems.Union(other.TransactionItems).Count() == TransactionItems.Count();
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj))
            {
                return false;
            }
            return obj is Transaction<TValue> && Equals((Transaction<TValue>) obj);
        }

        public override int GetHashCode()
        {
            unchecked
            {
                return ((TransactionKey?.GetHashCode() ?? 0)*397) ^
                       (TransactionItems?.Aggregate(397, (acc, elem) => acc ^ elem.GetHashCode()) ?? 0);
            }
        }

        public static bool operator ==(Transaction<TValue> left, Transaction<TValue> right)
        {
            return left.Equals(right);
        }

        public static bool operator !=(Transaction<TValue> left, Transaction<TValue> right)
        {
            return !left.Equals(right);
        }
    }
}