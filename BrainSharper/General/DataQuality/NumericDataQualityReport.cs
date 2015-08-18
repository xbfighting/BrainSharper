namespace BrainSharper.General.DataQuality
{
    using System.Collections.Generic;

    public class NumericDataQualityReport : IDataQualityReport<double>
    {
        public NumericDataQualityReport(IList<double> expectedValues, IList<double> actualValues, int casesCount, double accuracy)
        {
            this.ExpectedValues = expectedValues;
            this.ActualValues = actualValues;
            this.CasesCount = casesCount;
            this.Accuracy = accuracy;
        }

        public IList<double> ExpectedValues { get; }
        public IList<double> ActualValues { get; }
        public int CasesCount { get; }
        public double Accuracy { get; }
    }
}
