using BrainSharper.Abstract.Algorithms.Knn;

namespace BrainSharper.Implementations.Algorithms.Knn
{
    public struct KnnAdditionalParams : IKnnAdditionalParams
    {
        public KnnAdditionalParams(int kNeighbors, bool useWeightedDistances)
        {
            KNeighbors = kNeighbors;
            UseWeightedDistances = useWeightedDistances;
        }

        public int KNeighbors { get; }
        public bool UseWeightedDistances { get; }
    }
}