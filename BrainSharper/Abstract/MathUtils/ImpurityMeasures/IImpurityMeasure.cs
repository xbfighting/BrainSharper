using System.Collections.Generic;

namespace BrainSharper.Abstract.MathUtils.ImpurityMeasures
{
    public interface IImpurityMeasure<T>
    {
        double ImpurityValue(IList<T> values);
        double ImpurityValue(IList<int> elementsInGroupsCount);
    }
}
