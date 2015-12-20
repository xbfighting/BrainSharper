using BrainSharper.Abstract.Algorithms.DecisionTrees.DataStructures.BinaryTrees;

namespace BrainSharper.Implementations.Algorithms.DecisionTrees.DataStructures.BinaryDecisionTrees
{
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
            if (obj.GetType() != GetType()) return false;
            return Equals((BinarySplittingParams) obj);
        }

        public override int GetHashCode()
        {
            unchecked
            {
                return ((SplitOnFeature?.GetHashCode() ?? 0)*397) ^ SplitOnValue.GetHashCode();
            }
        }

        protected bool Equals(BinarySplittingParams other)
        {
            return string.Equals(SplitOnFeature, other.SplitOnFeature) && Equals(SplitOnValue, other.SplitOnValue);
        }
    }
}