namespace BrainSharper.Implementations.Algorithms.AssociationAnalysis.DataStructures
{
    using System.Collections.Generic;
    using System.Linq;

    using Abstract.Algorithms.AssociationAnalysis.DataStructures;

    public class FrequentItemsSet<TValue> : IFrequentItemsSet<TValue>
    {
        public FrequentItemsSet(
            double support, 
            double relativeSuppot, 
            IEnumerable<object> transactionIds, 
            IEnumerable<TValue> items)
        {
            ItemsSet = new SortedSet<TValue>(items);
            OrderedItems = ItemsSet.ToList();
            Support = support;
            TransactionIds = new HashSet<object>(transactionIds);
            RelativeSuppot = relativeSuppot;
        }

        public FrequentItemsSet(
            double support,
            double relativeSuppot,
            IEnumerable<object> transactionIds,
            params TValue[] items)
            : this(support, relativeSuppot, transactionIds, items.AsEnumerable())
        {
            
        }

        public ISet<TValue> ItemsSet { get; }
        public IList<TValue> OrderedItems { get; }
        public double Support { get; }
        public double RelativeSuppot { get; }
        public ISet<object> TransactionIds { get; }

        protected bool Equals(FrequentItemsSet<TValue> other)
        {
            return this.ItemsSet.SetEquals(other.ItemsSet) && Support.Equals(other.Support) && RelativeSuppot.Equals(other.RelativeSuppot) && TransactionIds.SetEquals(other.TransactionIds);
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj))
            {
                return false;
            }
            if (ReferenceEquals(this, obj))
            {
                return true;
            }
            if (obj.GetType() != this.GetType())
            {
                return false;
            }
            return Equals((FrequentItemsSet<TValue>)obj);
        }

        public override int GetHashCode()
        {
            unchecked
            {
                var hashCode = this.ItemsSet.Aggregate(397, (acc, itm) => acc + itm.GetHashCode());
                hashCode = (hashCode * 397) ^ this.Support.GetHashCode();
                hashCode = (hashCode * 397) ^ this.RelativeSuppot.GetHashCode();
                hashCode = (hashCode * 397) ^ TransactionIds.Aggregate(397, (acc, itm) => acc + itm.GetHashCode());
                return hashCode;
            }
        }

        public static bool operator ==(FrequentItemsSet<TValue> left, FrequentItemsSet<TValue> right)
        {
            return Equals(left, right);
        }

        public static bool operator !=(FrequentItemsSet<TValue> left, FrequentItemsSet<TValue> right)
        {
            return !Equals(left, right);
        }
    }
}
