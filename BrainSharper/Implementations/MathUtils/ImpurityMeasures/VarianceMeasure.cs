namespace BrainSharper.Implementations.MathUtils.ImpurityMeasures
{
    using System.Collections.Generic;
    using Abstract.MathUtils.ImpurityMeasures;
    using MathNet.Numerics.Statistics;

    public class VarianceMeasure : IImpurityMeasure<double>
    {
        public double ImpurityValue(IList<double> values)
        {
            return 1 / values.Variance();
        }
    }
}
