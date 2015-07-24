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
    }
}
