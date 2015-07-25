using BrainSharper.Abstract.Algorithms.DecisionTrees.DataStructures;

namespace BrainSharper.Implementations.Algorithms.DecisionTrees.DataStructures
{
    public class DecisionLink<TTestResult> : IDecisionTreeLink<TTestResult>
    {
        public DecisionLink(double instancesPercentage, long instancesCount, TTestResult testResult)
        {
            InstancesPercentage = instancesPercentage;
            InstancesCount = instancesCount;
            TestResult = testResult;
        }

        public double InstancesPercentage { get; }
        public long InstancesCount { get; }
        public TTestResult TestResult { get; }

        protected bool Equals(DecisionLink<TTestResult> other)
        {
            return InstancesPercentage.Equals(other.InstancesPercentage) && InstancesCount == other.InstancesCount;
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != this.GetType()) return false;
            return Equals((DecisionLink<TTestResult>) obj);
        }

        public override int GetHashCode()
        {
            unchecked
            {
                return (InstancesPercentage.GetHashCode()*397) ^ InstancesCount.GetHashCode();
            }
        }
    }
}
