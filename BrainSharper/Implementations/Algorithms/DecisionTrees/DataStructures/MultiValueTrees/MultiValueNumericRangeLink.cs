using BrainSharper.Abstract.Algorithms.DecisionTrees.DataStructures.MultiValueTrees;

namespace BrainSharper.Implementations.Algorithms.DecisionTrees.DataStructures.MultiValueTrees
{
    public class MultiValueNumericRangeLink : IMultiValueNumericRangeLink
    {
        public MultiValueNumericRangeLink(
            double instancesPercentage,
            long instancesCount,
            double testResult,
            double rangeStart,
            double rangeEnd)
        {
            InstancesPercentage = instancesPercentage;
            InstancesCount = instancesCount;
            TestResult = testResult;
            RangeStart = rangeStart;
            RangeEnd = rangeEnd;
        }

        public double InstancesPercentage { get; }
        public long InstancesCount { get; }
        public object TestResult { get; }
        public double RangeStart { get; }
        public double RangeEnd { get; }

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
            return Equals((MultiValueNumericRangeLink) obj);
        }

        public override int GetHashCode()
        {
            unchecked
            {
                var hashCode = InstancesPercentage.GetHashCode();
                hashCode = (hashCode*397) ^ InstancesCount.GetHashCode();
                hashCode = (hashCode*397) ^ TestResult.GetHashCode();
                hashCode = (hashCode*397) ^ RangeEnd.GetHashCode();
                hashCode = (hashCode*397) ^ RangeStart.GetHashCode();
                return hashCode;
            }
        }

        protected bool Equals(MultiValueNumericRangeLink other)
        {
            return InstancesPercentage.Equals(other.InstancesPercentage)
                   && InstancesCount == other.InstancesCount && TestResult.Equals(other.TestResult)
                   && RangeEnd.Equals(other.RangeEnd) && RangeStart.Equals(other.RangeStart);
        }
    }
}