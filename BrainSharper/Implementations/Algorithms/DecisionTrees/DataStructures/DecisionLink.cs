using BrainSharper.Abstract.Algorithms.DecisionTrees.DataStructures;

namespace BrainSharper.Implementations.Algorithms.DecisionTrees.DataStructures
{
    public class DecisionLink : IDecisionTreeLink
    {
        public DecisionLink(double instancesPercentage, long instancesCount, object testResult)
        {
            InstancesPercentage = instancesPercentage;
            InstancesCount = instancesCount;
            TestResult = testResult;
        }

        public double InstancesPercentage { get; }
        public long InstancesCount { get; }
        public object TestResult { get; }

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
            return Equals((DecisionLink) obj);
        }

        public override int GetHashCode()
        {
            unchecked
            {
                var hashCode = InstancesPercentage.GetHashCode();
                hashCode = (hashCode*397) ^ InstancesCount.GetHashCode();
                hashCode = (hashCode*397) ^ TestResult.GetHashCode();
                return hashCode;
            }
        }

        protected bool Equals(DecisionLink other)
        {
            return InstancesPercentage.Equals(other.InstancesPercentage) && InstancesCount == other.InstancesCount &&
                   Equals(TestResult, other.TestResult);
        }
    }
}