namespace BrainSharper.General.DataQuality
{
    using System.Collections.Generic;

    public interface IDataQualityReport<TPredictionResult>
    {
        IList<TPredictionResult> ExpectedValues { get; }
        IList<TPredictionResult> ActualValues { get; }
        int CasesCount { get; }
        double Accuracy { get; }
    }
}
