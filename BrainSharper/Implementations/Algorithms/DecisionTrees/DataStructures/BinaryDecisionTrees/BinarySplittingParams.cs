namespace BrainSharper.Implementations.Algorithms.DecisionTrees.DataStructures.BinaryDecisionTrees
{
    using System.Collections.Generic;
    using Abstract.Algorithms.DecisionTrees.DataStructures.BinaryTrees;

    public class BinarySplittingParams<T> : SplittingParams<T>, IBinarySplittingParams<T>
    {
        public BinarySplittingParams(string splitOnFeature, T splitOnValue, string dependentFeatureName)
            : base(splitOnFeature, dependentFeatureName)
        {
            this.SplitOnValue = splitOnValue;
        }

        public T SplitOnValue { get; }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != this.GetType()) return false;
            return this.Equals((BinarySplittingParams<T>) obj);
        }

        public override int GetHashCode()
        {
            unchecked
            {
                return ((this.SplitOnFeature?.GetHashCode() ?? 0)*397) ^ EqualityComparer<T>.Default.GetHashCode(this.SplitOnValue);
            }
        }

        protected bool Equals(BinarySplittingParams<T> other)
        {
            return string.Equals(this.SplitOnFeature, other.SplitOnFeature) && EqualityComparer<T>.Default.Equals(this.SplitOnValue, other.SplitOnValue);
        }
    }
}
