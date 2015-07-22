using System.Collections.Generic;
using BrainSharper.Abstract.MathUtils.ImpurityMeasures;
using MathNet.Numerics.Statistics;

namespace BrainSharper.Implementations.MathUtils.ImpurityMeasures
{
    public class VarianceMeasure : IImpurityMeasure<double>
    {
        public double ImpurityValue(IList<double> values)
        {
            return values.Variance();
        }
    }
}
