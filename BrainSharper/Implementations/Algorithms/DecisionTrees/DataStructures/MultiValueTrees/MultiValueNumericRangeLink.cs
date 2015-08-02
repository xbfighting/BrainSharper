namespace BrainSharper.Implementations.Algorithms.DecisionTrees.DataStructures.MultiValueTrees
{
    using Abstract.Algorithms.DecisionTrees.DataStructures.MultiValueTrees;

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
            if (obj.GetType() != this.GetType())
            {
                return false;
            }
            return this.Equals((MultiValueNumericRangeLink)obj);
        }

        public override int GetHashCode()
        {
            unchecked
            {
                var hashCode = this.InstancesPercentage.GetHashCode();
                hashCode = (hashCode * 397) ^ this.InstancesCount.GetHashCode();
                hashCode = (hashCode * 397) ^ this.TestResult.GetHashCode();
                hashCode = (hashCode * 397) ^ this.RangeEnd.GetHashCode();
                hashCode = (hashCode * 397) ^ this.RangeStart.GetHashCode();
                return hashCode;
            }
        }

        protected bool Equals(MultiValueNumericRangeLink other)
        {
            return this.InstancesPercentage.Equals(other.InstancesPercentage)
                   && this.InstancesCount == other.InstancesCount && this.TestResult.Equals(other.TestResult)
                   && this.RangeEnd.Equals(other.RangeEnd) && this.RangeStart.Equals(other.RangeStart);
        }
    }
}
