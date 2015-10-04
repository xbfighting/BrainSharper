namespace BrainSharper.Implementations.Algorithms.RuleInduction.DataStructures
{
    using System;

    using BrainSharper.Abstract.Algorithms.RuleInduction.DataStructures;
    using BrainSharper.Abstract.Data;

    public class RangeSelector<TValue> : IRangeSelector<TValue>
    {
        public RangeSelector(
            string attributeName, 
            double rangeFrom, 
            double rangeTo,
            bool fromInclusive = false,
            bool toInclusive = false)
        {
            this.AttributeName = attributeName;
            this.RangeFrom = rangeFrom;
            this.FromInclusive = fromInclusive;
            this.RangeTo = rangeTo;
            this.ToInclusive = toInclusive;
        }

        public bool IsUniversal => false;

        public bool IsEmpty => false;

        public string AttributeName { get; }

        public bool Covers(IDataVector<TValue> example)
        {
            if (!example.FeatureNames.Contains(this.AttributeName))
            {
                return false;
            }

            double featureValue = (double)Convert.ChangeType(example[this.AttributeName], typeof(double));
            if (featureValue >= this.RangeFrom && featureValue <= this.RangeTo)
            {
                if (featureValue == this.RangeFrom && !this.FromInclusive)
                {
                    return false;
                }

                if (featureValue == this.RangeTo && !this.ToInclusive)
                {
                    return false;
                }

                return true;
            }
            return false;
        }

        public ISelector<TValue> Intersect(ISelector<TValue> other)
        {
            if (other.IsUniversal)
            {
                return this;
            }

            if (other.IsEmpty)
            {
                return other;
            }

            if (!(other is IRangeSelector<TValue>))
            {
                throw new ArgumentException($"Cannot intersect range selector with {other.GetType().Name}");
            }

            //TODO: implement numeric selectors intersections
            throw new NotImplementedException("Implement me");
        }

        public bool IsMoreGeneralThan(ISelector<TValue> other)
        {
            throw new NotImplementedException();
        }

        public double RangeFrom { get; }

        public bool FromInclusive { get; }

        public double RangeTo { get; }

        public bool ToInclusive { get; }
    }
}