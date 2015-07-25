using System.Collections.Generic;
using BrainSharper.Abstract.Algorithms.DecisionTrees.DataStructures.BinaryTrees;

namespace BrainSharper.Implementations.Algorithms.DecisionTrees.DataStructures.BinaryDecisionTrees
{
    public class BinarySplittingParams<T> : SplittingParams<T>, IBinarySplittingParams<T>
    {
        public BinarySplittingParams(string splitOnFeature, T splitOnValue)
            : base(splitOnFeature)
        {
            SplitOnValue = splitOnValue;
        }

        public T SplitOnValue { get; }

        protected bool Equals(BinarySplittingParams<T> other)
        {
            return string.Equals(SplitOnFeature, other.SplitOnFeature) && EqualityComparer<T>.Default.Equals(SplitOnValue, other.SplitOnValue);
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != this.GetType()) return false;
            return Equals((BinarySplittingParams<T>) obj);
        }

        public override int GetHashCode()
        {
            unchecked
            {
                return ((SplitOnFeature?.GetHashCode() ?? 0)*397) ^ EqualityComparer<T>.Default.GetHashCode(SplitOnValue);
            }
        }
    }
}
