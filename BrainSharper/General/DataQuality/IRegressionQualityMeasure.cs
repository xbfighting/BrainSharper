namespace BrainSharper.General.DataQuality
{
    public interface IRegressionQualityMeasure : IDataQualityReport<double>
    {
        double RSquared { get; }
        double ErrorRate { get; }
    }
}