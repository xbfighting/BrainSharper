using BrainSharper.Abstract.Algorithms.Infrastructure;

namespace BrainSharper.Abstract.Algorithms.Knn
{
    public interface IKnnAdditionalParams : IModelBuilderParams
    {
        int KNeighbors { get; }
        bool UseWeightedDistances { get; }
    }
}
