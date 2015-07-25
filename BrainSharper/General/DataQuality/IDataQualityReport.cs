using System.Collections.Generic;

namespace BrainSharper.General.DataQuality
{
    public interface IDataQualityReport<TPredictionResult>
    {
        IList<TPredictionResult> ExpectedValues { get; }
        IList<TPredictionResult> ActualValues { get; }
        int CasesCount { get; }
        double Accuracy { get; }
    }
}
