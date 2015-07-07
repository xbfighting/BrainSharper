using System.Collections.Generic;

namespace BrainSharper.Abstract.Algorithms.Knn.BackwardsElimination
{
    public interface IBackwardsEliminationKnnModel<TPredictionResult> : IKnnPredictionModel<TPredictionResult>
    {
        IList<IBackwardsEliminationRemovedFeatureData> RemovedFeaturesData { get; }
    }
}