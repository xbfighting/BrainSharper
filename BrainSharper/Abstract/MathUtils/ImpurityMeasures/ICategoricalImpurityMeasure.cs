using System.Collections.Generic;

namespace BrainSharper.Abstract.MathUtils.ImpurityMeasures
{
    public interface ICategoricalImpurityMeasure<T> : IImpurityMeasure<T>
    {
        double ImpurityValue(IList<int> elementsInGroupsCount);
    }
}
