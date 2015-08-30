namespace BrainSharper.General.DataQuality
{
    using System.Collections.Generic;

    public interface IDataQualityMeasure<TPredictionResult>
    {
        double CalculateError(IList<TPredictionResult> expected, IList<TPredictionResult> actual);
        IDataQualityReport<TPredictionResult> GetReport(IList<TPredictionResult> expected, IList<TPredictionResult> actual);
    }
}
