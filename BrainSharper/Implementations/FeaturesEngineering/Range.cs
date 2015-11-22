namespace BrainSharper.Implementations.FeaturesEngineering
{
    using System;

    using Abstract.Data;
    using Abstract.FeaturesEngineering;

    public class Range : IRange
    {
        public Range(
            string attributeName, 
            double rangeFrom, 
            double rangeTo)
        {
            this.AttributeName = attributeName;
            this.RangeFrom = rangeFrom;
            this.RangeTo = rangeTo;
        }

        public bool IsUniversal => double.IsNegativeInfinity(this.RangeFrom) && double.IsPositiveInfinity(this.RangeTo);

        public string AttributeName { get; }

        public double RangeFrom { get; }

        public double RangeTo { get; }

        public bool ValuesRangeOverlap(IRange otherRange)
        {
            if (otherRange == null)
            {
                return false;
            }

            if (otherRange.AttributeName != this.AttributeName)
            {
                return false;
            }

            if (otherRange.IsUniversal)
            {
                return true;
            }
            return (otherRange.RangeFrom >= this.RangeFrom && otherRange.RangeFrom < this.RangeTo) ||
                   (otherRange.RangeTo > this.RangeFrom || otherRange.RangeTo <= this.RangeTo);
        }

        public bool Covers<T>(IDataVector<T> example)
        {
            if (!example.FeatureNames.Contains(this.AttributeName))
            {
                return false;
            }

            double featureValue = (double)Convert.ChangeType(example[this.AttributeName], typeof(double));
            return featureValue >= this.RangeFrom && featureValue < this.RangeTo;
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

            return otherRange.RangeFrom > this.RangeFrom && otherRange.RangeTo <= this.RangeTo;
        }

        protected bool Equals(Range other)
        {
            return string.Equals(this.AttributeName, other.AttributeName) && this.RangeFrom.Equals(other.RangeFrom) && this.RangeTo.Equals(other.RangeTo);
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
            if (obj.GetType() != this.GetType())
            {
                return false;
            }
            return Equals((Range)obj);
        }

        public override int GetHashCode()
        {
            unchecked
            {
                var hashCode = (this.AttributeName != null ? this.AttributeName.GetHashCode() : 0);
                hashCode = (hashCode * 397) ^ this.RangeFrom.GetHashCode();
                hashCode = (hashCode * 397) ^ this.RangeTo.GetHashCode();
                return hashCode;
            }
        }
    }
}