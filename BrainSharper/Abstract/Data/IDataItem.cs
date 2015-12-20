using System;

namespace BrainSharper.Abstract.Data
{
    public interface IDataItem<TValue> : IComparable<IDataItem<TValue>>
    {
        string FeatureName { get; }
        TValue FeatureValue { get; }
    }
}