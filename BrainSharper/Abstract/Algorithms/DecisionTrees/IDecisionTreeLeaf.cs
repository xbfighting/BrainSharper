namespace BrainSharper.Abstract.Algorithms.DecisionTrees
{
    public interface IDecisionTreeLeaf<TDecisionValue> : IDecisionTreeNode
    {
        TDecisionValue LeafValue { get; }
    }
}
