namespace BrainSharper.Implementations.Algorithms.DecisionTrees.DataStructures
{
    using System.Collections.Generic;

    using BrainSharper.Abstract.Algorithms.DecisionTrees.DataStructures;

    public class DecisionLink<TTestResult> : IDecisionTreeLink<TTestResult>
    {
        public DecisionLink(double instancesPercentage, long instancesCount, TTestResult testResult)
        {
            this.InstancesPercentage = instancesPercentage;
            this.InstancesCount = instancesCount;
            this.TestResult = testResult;
        }

        public double InstancesPercentage { get; }

        public long InstancesCount { get; }

        public TTestResult TestResult { get; }

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
            return this.Equals((DecisionLink<TTestResult>)obj);
        }

        public override int GetHashCode()
        {
            unchecked
            {
                var hashCode = this.InstancesPercentage.GetHashCode();
                hashCode = (hashCode * 397) ^ this.InstancesCount.GetHashCode();
                hashCode = (hashCode * 397) ^ EqualityComparer<TTestResult>.Default.GetHashCode(this.TestResult);
                return hashCode;
            }
        }

        protected bool Equals(DecisionLink<TTestResult> other)
        {
            return this.InstancesPercentage.Equals(other.InstancesPercentage) && this.InstancesCount == other.InstancesCount && EqualityComparer<TTestResult>.Default.Equals(this.TestResult, other.TestResult);
        }

    }
}
