using System.Collections.Generic;
using BrainSharper.Abstract.Data;

namespace BrainSharper.Implementations.Data
{
    public class DataItem<TValue> : IDataItem<TValue>
    {
        public DataItem(string featureName, TValue value)
        {
            FeatureName = featureName;
            FeatureValue = value;
        }

        public string FeatureName { get; }

        public TValue FeatureValue { get; }

        protected bool Equals(DataItem<TValue> other)
        {
            return string.Equals(FeatureName, other.FeatureName) && EqualityComparer<TValue>.Default.Equals(FeatureValue, other.FeatureValue);
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != this.GetType()) return false;
            return Equals((DataItem<TValue>) obj);
        }

        public override int GetHashCode()
        {
            unchecked
            {
                return ((FeatureName?.GetHashCode() ?? 0)*397) ^ EqualityComparer<TValue>.Default.GetHashCode(FeatureValue);
            }
        }
    }
}
