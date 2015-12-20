using System;
using System.Collections.Generic;

namespace BrainSharper.Abstract.Algorithms.DecisionTrees.DataStructures
{
    public interface IDecisionTreeParentNode : IDecisionTreeNode
    {
        IList<IDecisionTreeNode> Children { get; }
        IList<Tuple<IDecisionTreeLink, IDecisionTreeNode>> ChildrenWithTestResults { get; }
        IDecisionTreeNode GetChildForTestResult(object testResult);
        IDecisionTreeNode GetChildForLink(IDecisionTreeLink link);
        object GetTestResultForChild(IDecisionTreeNode child);
        IDecisionTreeLink GetChildLinkForChild(IDecisionTreeNode child);
        bool TestResultsContains(object testResult);
    }
}