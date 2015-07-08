namespace BrainSharper.Abstract.Algorithms.DecisionTrees.MultiValueTrees
{
    public interface IDecisionTreeMultiValueLink<TDecisionValue> : IDecisionTreeLink
    {
        TDecisionValue LinkDecisionValue { get; }
    }
}
