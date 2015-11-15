namespace BrainSharper.Abstract.MathUtils.ImpurityMeasures
{
    using System.Collections.Generic;

    public interface IImpurityMeasure<T>
    {
        double ImpurityValue(IList<T> values);
    }
}
