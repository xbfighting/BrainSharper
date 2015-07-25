using BrainSharper.Abstract.Algorithms.DecisionTrees.DataStructures;

namespace BrainSharper.Implementations.Algorithms.DecisionTrees.DataStructures
{
    public class SplittingParams<TValue> : ISplittingParams
    {
        public SplittingParams(string splitOnFeature)
        {
            SplitOnFeature = splitOnFeature;
        }

        public string SplitOnFeature { get; }

        protected bool Equals(SplittingParams<TValue> other)
        {
            return string.Equals(SplitOnFeature, other.SplitOnFeature);
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != this.GetType()) return false;
            return Equals((SplittingParams<TValue>) obj);
        }

        public override int GetHashCode()
        {
            return SplitOnFeature?.GetHashCode() ?? 0;
        }
    }
}
