namespace BrainSharper.Abstract.Algorithms.Knn.BackwardsElimination
{
    public interface IBackwardsEliminationRemovedFeatureData
    {
        string FeatureName { get; }
        double ErrorGain { get; }
    }
}