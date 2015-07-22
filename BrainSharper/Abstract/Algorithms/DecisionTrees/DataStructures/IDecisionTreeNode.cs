using BrainSharper.Abstract.Algorithms.Infrastructure;

namespace BrainSharper.Abstract.Algorithms.DecisionTrees.DataStructures
{
    public interface IDecisionTreeNode : IPredictionModel
    {
        bool IsLeaf { get; }
        string DecisionFeatureName { get; }
    }
}
