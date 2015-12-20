using System;
using BrainSharper.Abstract.Data;
using BrainSharper.Abstract.FeaturesEngineering;

namespace BrainSharper.Implementations.FeaturesEngineering
{
    public class Range : IRange
    {
        public Range(
            string attributeName,
            double rangeFrom,
            double rangeTo)
        {
            AttributeName = attributeName;
            RangeFrom = rangeFrom;
            RangeTo = rangeTo;
        }

        public bool IsUniversal => double.IsNegativeInfinity(RangeFrom) && double.IsPositiveInfinity(RangeTo);
        public string AttributeName { get; }
        public double RangeFrom { get; }
        public double RangeTo { get; }

        public bool ValuesRangeOverlap(IRange otherRange)
        {
            if (otherRange == null)
            {
                return false;
            }

            if (otherRange.AttributeName != AttributeName)
            {
                return false;
            }

            if (otherRange.IsUniversal)
            {
                return true;
            }
            return (otherRange.RangeFrom >= RangeFrom && otherRange.RangeFrom < RangeTo) ||
                   (otherRange.RangeTo > RangeFrom || otherRange.RangeTo <= RangeTo);
        }

        public bool Covers<T>(IDataVector<T> example)
        {
            if (!example.FeatureNames.Contains(AttributeName))
            {
                return false;
            }

            var featureValue = (double) Convert.ChangeType(example[AttributeName], typeof (double));
            return featureValue >= RangeFrom && featureValue < RangeTo;
        }

        public IRange Intersect(IRange other)
        {
            if (other.IsUniversal)
            {
                return this;
            }

            //TODO: implement numeric selectors intersections
            throw new NotImplementedException("Implement me");
        }

        public bool IsMoreGeneralThan(IRange otherRange)
        {
            if (otherRange == null)
            {
                return false;
            }

            return otherRange.RangeFrom > RangeFrom && otherRange.RangeTo <= RangeTo;
        }

        protected bool Equals(Range other)
        {
            return string.Equals(AttributeName, other.AttributeName) && RangeFrom.Equals(other.RangeFrom) &&
                   RangeTo.Equals(other.RangeTo);
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj))
            {
                return false;
            }
            if (ReferenceEquals(this, obj))
            {
                return true;
            }
            if (obj.GetType() != GetType())
            {
                return false;
            }
            return Equals((Range) obj);
        }

        public override int GetHashCode()
        {
            unchecked
            {
                var hashCode = (AttributeName != null ? AttributeName.GetHashCode() : 0);
                hashCode = (hashCode*397) ^ RangeFrom.GetHashCode();
                hashCode = (hashCode*397) ^ RangeTo.GetHashCode();
                return hashCode;
            }
        }
    }
}