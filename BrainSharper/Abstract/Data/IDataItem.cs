namespace BrainSharper.Abstract.Data
{
    using System;

    public interface IDataItem<TValue> : IComparable<IDataItem<TValue>>
    {
        string FeatureName { get; }
        TValue FeatureValue { get; }
    }
}