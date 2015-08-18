
namespace BrainSharper.General.DataQuality
{
    using System;
    using System.Collections.Generic;
    using System.Linq;

    using MathNet.Numerics;

    public class RootMeanSquareErrorQualityMeasure : IDataQualityMeasure<double>
    {
        public double MeasureAccuracy(IList<double> expected, IList<double> actual)
        {
            return Math.Sqrt(Distance.MSE(expected.ToArray(), actual.ToArray()));
        }

        public IDataQualityReport<double> GetReport(IList<double> expected, IList<double> actual)
        {
            return new NumericDataQualityReport(expected, actual, expected.Count, MeasureAccuracy(expected, actual));
        }
    }
}
