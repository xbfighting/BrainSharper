using BrainSharper.Abstract.Algorithms.Infrastructure;
using BrainSharper.Abstract.MathUtils.DistanceMeaseures;

namespace BrainSharper.Abstract.Algorithms.Knn
{
    public interface IKnnPredictor : IPredictor<double>
    {
        IDistanceMeasure DistanceMeasure { get; }
    }
}
