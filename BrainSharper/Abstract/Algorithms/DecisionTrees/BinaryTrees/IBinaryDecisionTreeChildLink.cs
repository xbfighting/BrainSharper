namespace BrainSharper.Abstract.Algorithms.DecisionTrees.BinaryTrees
{
    public interface IBinaryDecisionTreeChildLink : IDecisionTreeLink
    {
        bool TestValue { get; }
    }
}
