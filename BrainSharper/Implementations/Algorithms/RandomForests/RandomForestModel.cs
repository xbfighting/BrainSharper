namespace BrainSharper.Implementations.Algorithms.RandomForests
{
    using System.Collections.Generic;

    using Abstract.Algorithms.DecisionTrees.DataStructures;
    using Abstract.Algorithms.RandomForests;

    public class RandomForestModel : IRandomForestModel
    {
        public RandomForestModel(IList<IDecisionTreeNode> decisionTrees, IList<double> outOfBagErrors)
        {
            DecisionTrees = decisionTrees;
            OutOfBagErrors = outOfBagErrors;
        }

        public IList<IDecisionTreeNode> DecisionTrees { get; }
        public IList<double> OutOfBagErrors { get; }
    }
}
