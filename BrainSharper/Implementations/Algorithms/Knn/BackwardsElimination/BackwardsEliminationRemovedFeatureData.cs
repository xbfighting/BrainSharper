namespace BrainSharper.Implementations.Algorithms.Knn.BackwardsElimination
{
    public struct BackwardsEliminationRemovedFeatureData : IBackwardsEliminationRemovedFeatureData
    {
        public BackwardsEliminationRemovedFeatureData(double errorGain, string featureName)
        {
            ErrorGain = errorGain;
            FeatureName = featureName;
        }

        public string FeatureName { get; }
        public double ErrorGain { get; }

        public bool Equals(BackwardsEliminationRemovedFeatureData other)
        {
            return string.Equals(FeatureName, other.FeatureName) && ErrorGain.Equals(other.ErrorGain);
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            return obj is BackwardsEliminationRemovedFeatureData && Equals((BackwardsEliminationRemovedFeatureData) obj);
        }

        public override int GetHashCode()
        {
            unchecked
            {
                return ((FeatureName?.GetHashCode() ?? 0)*397) ^ ErrorGain.GetHashCode();
            }
        }
    }
}
