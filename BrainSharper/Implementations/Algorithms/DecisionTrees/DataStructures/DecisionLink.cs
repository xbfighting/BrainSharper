namespace BrainSharper.Implementations.Algorithms.DecisionTrees.DataStructures
{
    using System.Collections.Generic;

    using Abstract.Algorithms.DecisionTrees.DataStructures;

    public class DecisionLink : IDecisionTreeLink
    {
        public DecisionLink(double instancesPercentage, long instancesCount, object testResult)
        {
            this.InstancesPercentage = instancesPercentage;
            this.InstancesCount = instancesCount;
            this.TestResult = testResult;
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
            if (obj.GetType() != this.GetType())
            {
                return false;
            }
            return this.Equals((DecisionLink)obj);
        }

        public override int GetHashCode()
        {
            unchecked
            {
                var hashCode = this.InstancesPercentage.GetHashCode();
                hashCode = (hashCode * 397) ^ this.InstancesCount.GetHashCode();
                hashCode = (hashCode * 397) ^ this.TestResult.GetHashCode();
                return hashCode;
            }
        }

        protected bool Equals(DecisionLink other)
        {
            return this.InstancesPercentage.Equals(other.InstancesPercentage) && this.InstancesCount == other.InstancesCount && Equals(this.TestResult, other.TestResult);
        }

    }
}
