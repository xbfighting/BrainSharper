namespace BrainSharper.Abstract.Algorithms.DecisionTrees.DataStructures
{
    public interface IDecisionTreeLeaf : IDecisionTreeNode
    {
        object LeafValue { get; }
    }
}
