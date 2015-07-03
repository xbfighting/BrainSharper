using System.Collections.Generic;
using BrainSharper.Implementations.Algorithms.Knn;

namespace BrainSharper.Abstract.Algorithms.Knn
{
    public interface IBackwardsEliminationKnnModel : IKnnPredictionModel
    {
        IList<IBackwardsEliminationRemovedFeatureData> RemovedFeaturesData { get; }
    }
}