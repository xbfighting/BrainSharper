namespace BrainSharper.Abstract.Algorithms.RandomForests
{
    using System.Collections.Generic;

    using DecisionTrees.DataStructures;
    using Infrastructure;

    public interface IRandomForestModel : IPredictionModel
    {
        IList<IDecisionTreeNode> DecisionTrees { get; }
        IList<double> OutOfBagErrors { get; } 
    }
}
