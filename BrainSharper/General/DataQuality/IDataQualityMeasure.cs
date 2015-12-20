using System.Collections.Generic;

namespace BrainSharper.General.DataQuality
{
    public interface IDataQualityMeasure<TPredictionResult>
    {
        double CalculateError(IList<TPredictionResult> expected, IList<TPredictionResult> actual);

        IDataQualityReport<TPredictionResult> GetReport(IList<TPredictionResult> expected,
            IList<TPredictionResult> actual);
    }
}