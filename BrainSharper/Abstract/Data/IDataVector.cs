using System.Collections.Generic;
using MathNet.Numerics.LinearAlgebra;

namespace BrainSharper.Abstract.Data
{
    public delegate TValue VectorMemberwiseIndexOpertor<TValue>(int featureIndex, TValue initialValue);
    public delegate TValue VectorMemberwiseNameOpertor<TValue>(string featureName, TValue initialValue);

    public interface IDataVector<TValue> : IList<TValue>
    {
        IList<string> FeatureNames { get; }
        IList<TValue> Values { get; }
        TValue this[string featureName] { get; }
        IList<IDataItem<TValue>> DataItems { get; }
        Vector<double> NumericVector { get; }

        IDataVector<TValue> Set(int fetureIdx, TValue value);
        IDataVector<TValue> Set(string featureName, TValue value);
        IDataVector<TValue> MemberwiseSet(VectorMemberwiseIndexOpertor<TValue> setterAction);
        IDataVector<TValue> MemberwiseSet(VectorMemberwiseNameOpertor<TValue> setterAction);
    }
}
