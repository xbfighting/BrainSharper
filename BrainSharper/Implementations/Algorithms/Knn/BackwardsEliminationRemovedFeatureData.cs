namespace BrainSharper.Implementations.Algorithms.Knn
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
    }
}
