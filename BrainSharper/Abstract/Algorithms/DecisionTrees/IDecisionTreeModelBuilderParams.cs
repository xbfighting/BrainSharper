namespace BrainSharper.Abstract.Algorithms.DecisionTrees
{
    using Infrastructure;

    public interface IDecisionTreeModelBuilderParams : IModelBuilderParams
    {
        bool ProcessSubtreesCreationInParallel { get; }
    }
}
