namespace BrainSharper.Implementations.Algorithms.DecisionTrees.DataStructures
{
    using BrainSharper.Abstract.Algorithms.DecisionTrees.DataStructures;

    public class DecisionTreeLeaf : IDecisionTreeLeaf
    {
        public DecisionTreeLeaf(string decisionFeatureName, object leafValue)
        {
            this.DecisionFeatureName = decisionFeatureName;
            this.LeafValue = leafValue;
        }

        public bool IsLeaf => true;
        public string DecisionFeatureName { get; }
        public object LeafValue { get; }

        protected bool Equals(DecisionTreeLeaf other)
        {
            return string.Equals(this.DecisionFeatureName, other.DecisionFeatureName) && Equals(this.LeafValue, other.LeafValue);
        }

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
            return Equals((DecisionTreeLeaf)obj);
        }

        public override int GetHashCode()
        {
            unchecked
            {
                return ((this.DecisionFeatureName != null ? this.DecisionFeatureName.GetHashCode() : 0) * 397) ^ (this.LeafValue != null ? this.LeafValue.GetHashCode() : 0);
            }
        }
    }
}
