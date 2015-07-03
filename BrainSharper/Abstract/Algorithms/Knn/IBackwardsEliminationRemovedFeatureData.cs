namespace BrainSharper.Implementations.Algorithms.Knn
{
    public interface IBackwardsEliminationRemovedFeatureData
    {
        string FeatureName { get; }
        double ErrorGain { get; }
    }
}