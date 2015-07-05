using System.Collections.Generic;
using BrainSharper.Implementations.Algorithms.Knn;

namespace BrainSharper.Abstract.Algorithms.Knn
{
    public interface IBackwardsEliminationKnnModel<TPredictionResult> : IKnnPredictionModel<TPredictionResult>
    {
        IList<IBackwardsEliminationRemovedFeatureData> RemovedFeaturesData { get; }
    }
}