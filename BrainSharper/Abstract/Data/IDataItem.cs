namespace BrainSharper.Abstract.Data
{
    public interface IDataItem<out TValue>
    {
        string FeatureName { get; }

        TValue FeatureValue { get; }
    }
}