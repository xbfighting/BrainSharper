using System.Collections.Generic;

namespace BrainSharper.Abstract.Algorithms.DecisionTrees
{
    public interface IDecisionTreeNode
    {
        bool IsLeaf { get; }
        string DecisionFeatureName { get; }
    }
}
