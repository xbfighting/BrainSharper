using BrainSharper.Abstract.Algorithms.DecisionTrees.DataStructures;

namespace BrainSharper.Implementations.Algorithms.DecisionTrees.DataStructures
{
    public class SplittingParams : ISplittingParams
    {
        public SplittingParams(string splitOnFeature, string dependentFeatureName)
        {
            SplitOnFeature = splitOnFeature;
            DependentFeatureName = dependentFeatureName;
        }

        public string SplitOnFeature { get; }
        public string DependentFeatureName { get; }

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
            return Equals(obj as SplittingParams);
        }

        public override int GetHashCode()
        {
            unchecked
            {
                return ((SplitOnFeature?.GetHashCode() ?? 0)*397) ^ (DependentFeatureName?.GetHashCode() ?? 0);
            }
        }

        protected bool Equals(SplittingParams other)
        {
            return string.Equals(SplitOnFeature, other.SplitOnFeature) &&
                   string.Equals(DependentFeatureName, other.DependentFeatureName);
        }
    }
}