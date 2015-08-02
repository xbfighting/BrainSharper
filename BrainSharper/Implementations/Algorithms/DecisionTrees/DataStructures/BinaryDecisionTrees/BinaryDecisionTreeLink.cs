namespace BrainSharper.Implementations.Algorithms.DecisionTrees.DataStructures.BinaryDecisionTrees
{
    using Abstract.Algorithms.DecisionTrees.DataStructures.BinaryTrees;

    public class BinaryDecisionTreeLink : DecisionLink, IBinaryDecisionTreeLink
    {
        public BinaryDecisionTreeLink(
            double instancesPercentage, 
            long instancesCount, 
            bool testValue) 
            : base(instancesPercentage, instancesCount, testValue)
        {
            this.LogicalTestResult = testValue;
        }

        public bool LogicalTestResult { get; }

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
                return (base.GetHashCode() * 397) ^ this.TestResult.GetHashCode();
            }
        }

        protected bool Equals(BinaryDecisionTreeLink other)
        {
            return base.Equals(other) && this.TestResult.Equals(other.TestResult);
        }
    }
}
