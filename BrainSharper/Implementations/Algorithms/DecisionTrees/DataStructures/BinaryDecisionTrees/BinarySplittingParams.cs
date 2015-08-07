namespace BrainSharper.Implementations.Algorithms.DecisionTrees.DataStructures.BinaryDecisionTrees
{
    using System.Collections.Generic;
    using Abstract.Algorithms.DecisionTrees.DataStructures.BinaryTrees;

    public class BinarySplittingParams : SplittingParams, IBinarySplittingParams
    {
        public BinarySplittingParams(string splitOnFeature, object splitOnValue, string dependentFeatureName)
            : base(splitOnFeature, dependentFeatureName)
        {
            SplitOnValue = splitOnValue;
        }

        public object SplitOnValue { get; }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != this.GetType()) return false;
            return this.Equals((BinarySplittingParams) obj);
        }

        public override int GetHashCode()
        {
            unchecked
            {
                return ((this.SplitOnFeature?.GetHashCode() ?? 0) * 397) ^ SplitOnValue.GetHashCode();
            }
        }

        protected bool Equals(BinarySplittingParams other)
        {
            return string.Equals(this.SplitOnFeature, other.SplitOnFeature) && Equals(SplitOnValue, other.SplitOnValue);
        }
    }
}
