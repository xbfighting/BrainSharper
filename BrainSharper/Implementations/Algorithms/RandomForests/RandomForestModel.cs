using System.Collections.Generic;
using BrainSharper.Abstract.Algorithms.DecisionTrees.DataStructures;
using BrainSharper.Abstract.Algorithms.RandomForests;

namespace BrainSharper.Implementations.Algorithms.RandomForests
{
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