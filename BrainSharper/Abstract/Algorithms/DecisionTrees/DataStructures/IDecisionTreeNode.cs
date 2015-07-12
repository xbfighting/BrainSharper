namespace BrainSharper.Abstract.Algorithms.DecisionTrees.DataStructures
{
    public interface IDecisionTreeNode
    {
        bool IsLeaf { get; }
        string DecisionFeatureName { get; }
    }
}
