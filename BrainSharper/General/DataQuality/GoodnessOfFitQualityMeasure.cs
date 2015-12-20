using System;
using System.Collections.Generic;
using System.Linq;
using MathNet.Numerics;

namespace BrainSharper.General.DataQuality
{
    public class GoodnessOfFitQualityMeasure : IDataQualityMeasure<double>
    {
        public double CalculateError(IList<double> expected, IList<double> actual)
        {
            return Math.Sqrt(Distance.MSE(expected.ToArray(), actual.ToArray()));
        }

        public IDataQualityReport<double> GetReport(IList<double> expected, IList<double> actual)
        {
            return new RegressionQualityReport(
                expected,
                actual,
                expected.Count,
                GoodnessOfFit.RSquared(expected, actual),
                CalculateError(expected, actual));
        }
    }
}