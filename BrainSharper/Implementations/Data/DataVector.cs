using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using BrainSharper.Abstract.Data;
using MathNet.Numerics.LinearAlgebra;

namespace BrainSharper.Implementations.Data
{
    public class DataVector<TValue> : IDataVector<TValue>
    {
        private readonly Lazy<Vector<double>> _numericVector;
        private readonly IList<TValue> _values;

        public DataVector(IList<TValue> values, IList<string> featureNames)
        {
            _values = values;
            _numericVector =
                new Lazy<Vector<double>>(
                    () => Vector<double>.Build.Dense(_values.Select(val => Convert.ToDouble(val)).ToArray()));
            FeatureNames = featureNames;
        }

        public DataVector(IList<TValue> values, string featureName)
        {
            _values = values;
            _numericVector =
                new Lazy<Vector<double>>(
                    () => Vector<double>.Build.Dense(_values.Select(val => Convert.ToDouble(val)).ToArray()));
            FeatureNames = Enumerable.Repeat(featureName, _values.Count).ToList();
        }

        #region Getters/setters

        public IList<TValue> Values => new List<TValue>(_values);
        public IList<string> FeatureNames { get; }
        public int Count => Values.Count;
        public bool IsReadOnly => true;

        public TValue this[int index]
        {
            get { return _values[index]; }
            set { throw new NotSupportedException("Data vector is not supporting set at operation!"); }
        }

        public IList<IDataItem<TValue>> DataItems
        {
            get
            {
                var tmpThis = this;
                return (
                    from elem in tmpThis._values.Select((value, index) => new {Value = value, Index = index})
                    select new DataItem<TValue>(tmpThis.FeatureNames[elem.Index], elem.Value) as IDataItem<TValue>
                    ).ToList();
            }
        }

        public Vector<double> NumericVector => _numericVector.Value;

        public IDataVector<TValue> Set(int fetureIdx, TValue value)
        {
            var newValues = new List<TValue>(_values) {[fetureIdx] = value};
            return new DataVector<TValue>(newValues, new List<string>(FeatureNames));
        }

        public IDataVector<TValue> Set(string featureName, TValue value)
        {
            return Set(FeatureNames.IndexOf(featureName), value);
        }

        public IDataVector<TValue> MemberwiseSet(VectorMemberwiseIndexOpertor<TValue> setterAction)
        {
            var newValues = _values.Select((val, idx) => setterAction(idx, val)).ToList();
            return new DataVector<TValue>(newValues, new List<string>(FeatureNames));
        }

        public IDataVector<TValue> MemberwiseSet(VectorMemberwiseNameOpertor<TValue> setterAction)
        {
            var tmpThis = this;
            var newValues = tmpThis._values.Select((val, idx) => setterAction(tmpThis.FeatureNames[idx], val)).ToList();
            return new DataVector<TValue>(newValues, new List<string>(FeatureNames));
        }

        public TValue this[string featureName] => _values[FeatureNames.IndexOf(featureName)];

        #endregion Getters/setters

        #region IList members

        public IEnumerator<TValue> GetEnumerator()
        {
            return _values.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return _values.GetEnumerator();
        }

        public void Add(TValue item)
        {
            throw new NotSupportedException("Data vector is not supporting addition operation!");
        }

        public void Clear()
        {
            throw new NotSupportedException("Data vector is not supporting cleaning operation!");
        }

        public bool Contains(TValue item)
        {
            return _values.Contains(item);
        }

        public void CopyTo(TValue[] array, int arrayIndex)
        {
            _values.CopyTo(array, arrayIndex);
        }

        public bool Remove(TValue item)
        {
            throw new NotSupportedException("Data vector is not supporting removing operation!");
        }

        public int IndexOf(TValue item)
        {
            return _values.IndexOf(item);
        }

        public void Insert(int index, TValue item)
        {
            throw new NotSupportedException("Data vector is not supporting insert at operation!");
        }

        public void RemoveAt(int index)
        {
            throw new NotSupportedException("Data vector is not supporting remove at operation!");
        }

        #endregion IList members

        #region Equality members

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (obj.GetType() != GetType()) return false;
            return Equals((DataVector<TValue>) obj);
        }

        public override int GetHashCode()
        {
            unchecked
            {
                int? valuesSum = 0;
                foreach (var elem in _values)
                {
                    valuesSum += elem.GetHashCode();
                }

                int? fetureNamesSum = 0;
                foreach (var elem in FeatureNames)
                {
                    fetureNamesSum += elem.GetHashCode();
                }
                return ((valuesSum ?? 0)*397) ^ (fetureNamesSum ?? 0);
            }
        }

        private bool Equals(DataVector<TValue> other)
        {
            return (
                (_values == null && other._values == null) ||
                ((_values != null && other._values != null) && _values.SequenceEqual(other.Values))) &&
                   (
                       (FeatureNames == null && other.FeatureNames == null) ||
                       ((FeatureNames != null && other.FeatureNames != null) &&
                        FeatureNames.SequenceEqual(other.FeatureNames))
                       );
        }

        #endregion Equality members
    }
}