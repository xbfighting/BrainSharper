using BrainSharper.Abstract.Algorithms.Infrastructure;

namespace BrainSharper.Abstract.Algorithms.DecisionTrees
{
    public interface IDecisionTreeModelBuilderParams : IModelBuilderParams
    {
        bool ProcessSubtreesCreationInParallel { get; }
        bool UsePrunningHeuristicDuringTreeBuild { get; }
        int? MaximalTreeDepth { get; }
    }
}