using BrainSharper.Abstract.Algorithms.DecisionTrees;

namespace BrainSharper.Implementations.Algorithms.DecisionTrees
{
    public class DecisionLink : IDecisionTreeLink
    {
        public DecisionLink(double instancesPercentage, long instancesCount)
        {
            InstancesPercentage = instancesPercentage;
            InstancesCount = instancesCount;
        }

        public double InstancesPercentage { get; }
        public long InstancesCount { get; }

        protected bool Equals(DecisionLink other)
        {
            return InstancesPercentage.Equals(other.InstancesPercentage) && InstancesCount == other.InstancesCount;
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != this.GetType()) return false;
            return Equals((DecisionLink) obj);
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
