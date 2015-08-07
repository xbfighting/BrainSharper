namespace BrainSharper.Implementations.Algorithms.DecisionTrees.DataStructures
{
    using Abstract.Algorithms.DecisionTrees.DataStructures;

    public class SplittingParams : ISplittingParams
    {
        public SplittingParams(string splitOnFeature, string dependentFeatureName)
        {
            this.SplitOnFeature = splitOnFeature;
            this.DependentFeatureName = dependentFeatureName;
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
            if (obj.GetType() != this.GetType())
            {
                return false;
            }
            return Equals(obj as SplittingParams);
        }

        public override int GetHashCode()
        {
            unchecked
            {
                return ((this.SplitOnFeature?.GetHashCode() ?? 0) * 397) ^ (this.DependentFeatureName?.GetHashCode() ?? 0);
            }
        }

        protected bool Equals(SplittingParams other)
        {
            return string.Equals(this.SplitOnFeature, other.SplitOnFeature) && string.Equals(this.DependentFeatureName, other.DependentFeatureName);
        }
    }
}
