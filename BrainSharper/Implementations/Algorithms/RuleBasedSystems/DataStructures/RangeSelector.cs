namespace BrainSharper.Implementations.Algorithms.RuleBasedSystems.DataStructures
{
    using System;

    using Abstract.Algorithms.RuleBasedSystems.DataStructures;
    using Abstract.Data;

    public class RangeSelector : IRangeSelector
    {
        public RangeSelector(
            string attributeName, 
            double rangeFrom, 
            double rangeTo,
            bool fromInclusive = false,
            bool toInclusive = false)
        {
            AttributeName = attributeName;
            RangeFrom = rangeFrom;
            FromInclusive = fromInclusive;
            RangeTo = rangeTo;
            ToInclusive = toInclusive;
        }

        public bool IsUniversal => false;

        public bool IsEmpty => false;

        public string AttributeName { get; }

        public bool Covers<TValue>(IDataVector<TValue> example)
        {
            if (!example.FeatureNames.Contains(AttributeName))
            {
                return false;
            }

            double featureValue = (double)Convert.ChangeType(example[AttributeName], typeof(double));
            if (featureValue >= RangeFrom && featureValue <= RangeTo)
            {
                if (featureValue == RangeFrom && !FromInclusive)
                {
                    return false;
                }

                if (featureValue == RangeTo && !ToInclusive)
                {
                    return false;
                }

                return true;
            }
            return false;
        }

        public ISelector Intersect(ISelector other)
        {
            if (other.IsUniversal)
            {
                return this;
            }

            if (other.IsEmpty)
            {
                return other;
            }

            if (!(other is IRangeSelector))
            {
                throw new ArgumentException($"Cannot intersect range selector with {other.GetType().Name}");
            }

            //TODO: implement numeric selectors intersections
            throw new NotImplementedException("Implement me");
        }

        public bool IsMoreDetailedThan(ISelector other)
        {
            throw new NotImplementedException();
        }

        public double RangeFrom { get; }

        public bool FromInclusive { get; }

        public double RangeTo { get; }

        public bool ToInclusive { get; }
    }
}
