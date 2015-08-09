namespace BrainSharper.Implementations.Algorithms.DecisionTrees
{
    using BrainSharper.Abstract.Algorithms.DecisionTrees;

    public class DecisionTreeModelBuilderParams : IDecisionTreeModelBuilderParams
    {
        public DecisionTreeModelBuilderParams(bool processSubtreesCreationInParallel)
        {
            this.ProcessSubtreesCreationInParallel = processSubtreesCreationInParallel;
        }

        public bool ProcessSubtreesCreationInParallel { get; }
    }
}
