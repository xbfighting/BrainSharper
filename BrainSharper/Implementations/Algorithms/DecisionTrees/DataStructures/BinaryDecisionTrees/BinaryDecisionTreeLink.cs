using BrainSharper.Abstract.Algorithms.DecisionTrees.DataStructures.BinaryTrees;

namespace BrainSharper.Implementations.Algorithms.DecisionTrees.DataStructures.BinaryDecisionTrees
{
    public class BinaryDecisionTreeLink : DecisionLink, IBinaryDecisionTreeLink
    {
        public BinaryDecisionTreeLink(
            double instancesPercentage,
            long instancesCount,
            bool testValue)
            : base(instancesPercentage, instancesCount, testValue)
        {
            LogicalTestResult = testValue;
        }

        public bool LogicalTestResult { get; }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != GetType()) return false;
            return Equals((BinaryDecisionTreeLink) obj);
        }

        public override int GetHashCode()
        {
            unchecked
            {
                return (base.GetHashCode()*397) ^ TestResult.GetHashCode();
            }
        }

        protected bool Equals(BinaryDecisionTreeLink other)
        {
            return base.Equals(other) && TestResult.Equals(other.TestResult);
        }
    }
}