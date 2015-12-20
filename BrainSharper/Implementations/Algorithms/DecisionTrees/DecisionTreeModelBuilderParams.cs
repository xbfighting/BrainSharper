using BrainSharper.Abstract.Algorithms.DecisionTrees;

namespace BrainSharper.Implementations.Algorithms.DecisionTrees
{
    public class DecisionTreeModelBuilderParams : IDecisionTreeModelBuilderParams
    {
        public DecisionTreeModelBuilderParams(bool processSubtreesCreationInParallel,
            bool usePrunningHeuristicInBuild = false, int? maximalTreeDepth = null)
        {
            ProcessSubtreesCreationInParallel = processSubtreesCreationInParallel;
            UsePrunningHeuristicDuringTreeBuild = usePrunningHeuristicInBuild;
            MaximalTreeDepth = maximalTreeDepth;
        }

        public bool ProcessSubtreesCreationInParallel { get; }
        public bool UsePrunningHeuristicDuringTreeBuild { get; }
        public int? MaximalTreeDepth { get; }
    }
}