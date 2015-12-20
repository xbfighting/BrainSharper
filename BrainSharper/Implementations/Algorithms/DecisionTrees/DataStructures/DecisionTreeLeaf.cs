using BrainSharper.Abstract.Algorithms.DecisionTrees.DataStructures;

namespace BrainSharper.Implementations.Algorithms.DecisionTrees.DataStructures
{
    public class DecisionTreeLeaf : IDecisionTreeLeaf
    {
        public DecisionTreeLeaf(string decisionFeatureName, object leafValue)
        {
            DecisionFeatureName = decisionFeatureName;
            LeafValue = leafValue;
        }

        public bool IsLeaf => true;
        public string DecisionFeatureName { get; }
        public object LeafValue { get; }

        protected bool Equals(DecisionTreeLeaf other)
        {
            return string.Equals(DecisionFeatureName, other.DecisionFeatureName) && Equals(LeafValue, other.LeafValue);
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
            if (obj.GetType() != GetType())
            {
                return false;
            }
            return Equals((DecisionTreeLeaf) obj);
        }

        public override int GetHashCode()
        {
            unchecked
            {
                return ((DecisionFeatureName != null ? DecisionFeatureName.GetHashCode() : 0)*397) ^
                       (LeafValue != null ? LeafValue.GetHashCode() : 0);
            }
        }
    }
}