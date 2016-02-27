using System.Collections.Generic;
using System.Linq;
using BrainSharper.Abstract.Algorithms.AssociationAnalysis.DataStructures;

namespace BrainSharper.Implementations.Algorithms.AssociationAnalysis.DataStructures.Common
{
    public class FrequentItemsSet<TValue> : IFrequentItemsSet<TValue>
    {
        public FrequentItemsSet(
            ISet<TValue> itemsSet, 
            double support = 0.0, 
            double relativeSupport = 0.0)
            : this(new object[0], itemsSet, support, relativeSupport)
        {
        }

        public FrequentItemsSet(double support, double relativesupport, params TValue[] elements)
            : this(new HashSet<TValue>(elements), support, relativesupport)
        {
        }

        public FrequentItemsSet(
            IEnumerable<object> transactionIds,
            IEnumerable<TValue> items,
            double support = 0.0,
            double relativeSuppot = 0.0)
        {
            ItemsSet = new SortedSet<TValue>(items);
            OrderedItems = ItemsSet.ToList();
            Support = support;
            TransactionIds = new HashSet<object>(transactionIds);
            RelativeSupport = relativeSuppot;
        }

        public FrequentItemsSet(
            IEnumerable<object> transactionIds,
            ISet<TValue> itemsSet,
            double support = 0.0,
            double relativeSuppot = 0.0)
        {
            ItemsSet = new SortedSet<TValue>(itemsSet);
            OrderedItems = ItemsSet.ToList();
            Support = support;
            TransactionIds = new HashSet<object>(transactionIds);
            RelativeSupport = relativeSuppot;
        }

        public FrequentItemsSet(
            double support,
            double relativeSuppot,
            IEnumerable<object> transactionIds,
            params TValue[] items)
            : this(transactionIds, items.AsEnumerable(), support, relativeSuppot)
        {
        }

        public ISet<TValue> ItemsSet { get; }
        public IList<TValue> OrderedItems { get; }
        public double RelativeSupport { get; }
        public double Support { get; }
        public ISet<object> TransactionIds { get; }

        public bool KFirstElementsEqual(IFrequentItemsSet<TValue> other, int k)
        {
            return ItemsSet.Take(k).SequenceEqual(other.OrderedItems.Take(k));
        }

        public bool ItemsOnlyEqual(IFrequentItemsSet<TValue> other)
        {
            if (other == null)
            {
                return false;
            }
            return OrderedItems.SequenceEqual(other.OrderedItems);
        }

        protected bool Equals(FrequentItemsSet<TValue> other)
        {
            return ItemsSet.SetEquals(other.ItemsSet) && Support.Equals(other.Support) &&
                   RelativeSupport.Equals(other.RelativeSupport) && TransactionIds.SetEquals(other.TransactionIds);
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
            if (obj.GetType() != GetType())
            {
                return false;
            }
            return Equals((FrequentItemsSet<TValue>) obj);
        }

        public override int GetHashCode()
        {
            unchecked
            {
                var hashCode = ItemsSet.Aggregate(397, (acc, itm) => acc + itm.GetHashCode());
                hashCode = (hashCode*397) ^ Support.GetHashCode();
                hashCode = (hashCode*397) ^ RelativeSupport.GetHashCode();
                hashCode = (hashCode*397) ^ TransactionIds.Aggregate(397, (acc, itm) => acc + itm.GetHashCode());
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

        public override string ToString()
        {
            return $"Items: {string.Join(",", OrderedItems.Select(itm => $"({itm.ToString()})" ))} Support: { RelativeSupport} ";
        }
    }
}