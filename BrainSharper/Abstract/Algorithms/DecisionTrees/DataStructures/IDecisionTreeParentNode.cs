using System.Collections.Generic;

namespace BrainSharper.Abstract.Algorithms.DecisionTrees.DataStructures
{
    public interface IDecisionTreeParentNode<TTestResult> : IDecisionTreeNode
    {
        IList<IDecisionTreeNode> Children { get; }
        IDictionary<IDecisionTreeLink<TTestResult>, IDecisionTreeNode> LinksToChildren { get; }
        IDictionary<TTestResult, IDecisionTreeNode> ChildrenByTestResults { get; }
    }
}
