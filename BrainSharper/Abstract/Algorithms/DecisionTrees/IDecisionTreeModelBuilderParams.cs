namespace BrainSharper.Abstract.Algorithms.DecisionTrees
{
    using Infrastructure;

    public interface IDecisionTreeModelBuilderParams : IModelBuilderParams
    {
        bool ProcessSubtreesCreationInParallel { get; }
        bool UsePrunningHeuristicDuringTreeBuild { get; }
        int? MaximalTreeDepth { get; }
    }
}
