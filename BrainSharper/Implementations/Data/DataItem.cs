using System;
using System.Collections.Generic;
using System.Diagnostics;
using BrainSharper.Abstract.Data;

namespace BrainSharper.Implementations.Data
{
    using static String;

    [DebuggerDisplay("Feature: {FeatureName} Value: {FeatureValue}")]
    public struct DataItem<TValue> : IDataItem<TValue>
    {
        public DataItem(string featureName, TValue value)
        {
            FeatureName = featureName;
            FeatureValue = value;
        }

        public string FeatureName { get; }
        public TValue FeatureValue { get; }

        public int CompareTo(IDataItem<TValue> other)
        {
            if (other == null)
            {
                return 1;
            }
            var fieldNamesComparison = Compare(FeatureName, other.FeatureName, StringComparison.Ordinal);
            if (fieldNamesComparison == 0)
            {
                return Comparer<TValue>.Default.Compare(FeatureValue, other.FeatureValue);
            }
            return fieldNamesComparison;
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (obj.GetType() != GetType()) return false;
            return Equals((DataItem<TValue>) obj);
        }

        public override int GetHashCode()
        {
            unchecked
            {
                return ((FeatureName?.GetHashCode() ?? 0)*397) ^
                       EqualityComparer<TValue>.Default.GetHashCode(FeatureValue);
            }
        }

        private bool Equals(DataItem<TValue> other)
        {
            return string.Equals(FeatureName, other.FeatureName) &&
                   EqualityComparer<TValue>.Default.Equals(FeatureValue, other.FeatureValue);
        }

        public override string ToString()
        {
            return $"Feature: {FeatureName} Value: {FeatureValue}";
        }
    }
}