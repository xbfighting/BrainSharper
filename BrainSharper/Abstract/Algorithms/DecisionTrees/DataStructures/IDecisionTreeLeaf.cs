namespace BrainSharper.Abstract.Algorithms.DecisionTrees.DataStructures
{
    public interface IDecisionTreeLeaf<TDecisionValue> : IDecisionTreeNode
    {
        TDecisionValue LeafValue { get; }
    }
}
