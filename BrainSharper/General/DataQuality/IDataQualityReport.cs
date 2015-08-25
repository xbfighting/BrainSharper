namespace BrainSharper.General.DataQuality
{
    using System.Collections.Generic;

    public interface IDataQualityReport<TPredictionResult>
    {
        IList<TPredictionResult> ExpectedValues { get; }
        IList<TPredictionResult> ActualValues { get; }
        int CasesCount { get; }
        //TODO: later on split ot categorical and numeric quality separately: accuracy is for categorical data, rmse for numerical
        double Accuracy { get; }
    }
}
