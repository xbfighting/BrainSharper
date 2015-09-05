namespace BrainSharper.Implementations.Algorithms.DecisionTrees
{
    using BrainSharper.Abstract.Algorithms.DecisionTrees;

    public class DecisionTreeModelBuilderParams : IDecisionTreeModelBuilderParams
    {
        public DecisionTreeModelBuilderParams(bool processSubtreesCreationInParallel, bool usePrunningHeuristicInBuild = false, int? maximalTreeDepth = null)
        {
            this.ProcessSubtreesCreationInParallel = processSubtreesCreationInParallel;
            UsePrunningHeuristicDuringTreeBuild = usePrunningHeuristicInBuild;
            MaximalTreeDepth = maximalTreeDepth;
        }

        public bool ProcessSubtreesCreationInParallel { get; }
        public bool UsePrunningHeuristicDuringTreeBuild { get; }
        public int? MaximalTreeDepth { get; }
    }
}
