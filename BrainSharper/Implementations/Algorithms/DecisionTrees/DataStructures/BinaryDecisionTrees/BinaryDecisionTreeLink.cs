using BrainSharper.Abstract.Algorithms.DecisionTrees.DataStructures.BinaryTrees;

namespace BrainSharper.Implementations.Algorithms.DecisionTrees.DataStructures.BinaryDecisionTrees
{
    public class BinaryDecisionTreeLink : DecisionLink, IBinaryDecisionTreeLink
    {
        public BinaryDecisionTreeLink(
            double instancesPercentage, 
            long instancesCount, 
            bool testValue) 
            : base(instancesPercentage, instancesCount)
        {
            TestValue = testValue;
        }

        public bool TestValue { get; }

        protected bool Equals(BinaryDecisionTreeLink other)
        {
            return base.Equals(other) && TestValue.Equals(other.TestValue);
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != this.GetType()) return false;
            return Equals((BinaryDecisionTreeLink) obj);
        }

        public override int GetHashCode()
        {
            unchecked
            {
                return (base.GetHashCode()*397) ^ TestValue.GetHashCode();
            }
        }
    }
}
