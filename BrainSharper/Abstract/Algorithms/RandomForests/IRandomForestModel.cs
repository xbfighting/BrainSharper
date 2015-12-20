using System.Collections.Generic;
using BrainSharper.Abstract.Algorithms.DecisionTrees.DataStructures;
using BrainSharper.Abstract.Algorithms.Infrastructure;

namespace BrainSharper.Abstract.Algorithms.RandomForests
{
    public interface IRandomForestModel : IPredictionModel
    {
        IList<IDecisionTreeNode> DecisionTrees { get; }
        IList<double> OutOfBagErrors { get; }
    }
}