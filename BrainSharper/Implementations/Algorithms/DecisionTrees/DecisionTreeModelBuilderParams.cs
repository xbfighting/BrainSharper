namespace BrainSharper.Implementations.Algorithms.DecisionTrees
{
    using BrainSharper.Abstract.Algorithms.DecisionTrees;

    public class DecisionTreeModelBuilderParams : IDecisionTreeModelBuilderParams
    {
        public DecisionTreeModelBuilderParams(bool processSubtreesCreationInParallel, bool usePrunningHeuristicInBuild = false)
        {
            this.ProcessSubtreesCreationInParallel = processSubtreesCreationInParallel;
            UsePrunningHeuristicDuringTreeBuild = usePrunningHeuristicInBuild;
        }

        public bool ProcessSubtreesCreationInParallel { get; }
        public bool UsePrunningHeuristicDuringTreeBuild { get; }
    }
}
