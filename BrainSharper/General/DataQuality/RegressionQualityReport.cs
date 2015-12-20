using System.Collections.Generic;

namespace BrainSharper.General.DataQuality
{
    public class RegressionQualityReport : IRegressionQualityMeasure
    {
        public RegressionQualityReport(
            IList<double> expectedValues,
            IList<double> actualValues,
            int casesCount,
            double rsquared,
            double errorRate)
        {
            ExpectedValues = expectedValues;
            ActualValues = actualValues;
            CasesCount = casesCount;
            RSquared = rsquared;
            ErrorRate = errorRate;
        }

        public IList<double> ExpectedValues { get; }
        public IList<double> ActualValues { get; }
        public int CasesCount { get; }
        public double Accuracy => RSquared;
        public double RSquared { get; }
        public double ErrorRate { get; }
    }
}