using System.Collections.Generic;

namespace BrainSharper.General.DataQuality
{
    public class ConfusionMatrixBuilder<TPredictionResult> : IDataQualityMeasure<TPredictionResult>
    {
        public double CalculateError(IList<TPredictionResult> expected, IList<TPredictionResult> actual)
        {
            return 1 - (new ConfusionMatrix<TPredictionResult>(expected, actual)).Accuracy;
        }

        public IDataQualityReport<TPredictionResult> GetReport(IList<TPredictionResult> expected,
            IList<TPredictionResult> actual)
        {
            return new ConfusionMatrix<TPredictionResult>(expected, actual);
        }
    }
}