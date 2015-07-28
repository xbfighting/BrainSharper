namespace BrainSharper.Abstract.Algorithms.DecisionTrees.DataStructures
{
    using System;
    using System.Collections.Generic;

    public interface IDecisionTreeParentNode<TTestResult> : IDecisionTreeNode
    {
        IList<IDecisionTreeNode> Children { get; }

        IList<Tuple<IDecisionTreeLink<TTestResult>, IDecisionTreeNode>> ChildrenWithTestResults { get; } 
            
        IDecisionTreeNode GetChildForTestResult(TTestResult testResult);

        IDecisionTreeNode GetChildForLink(IDecisionTreeLink<TTestResult> link);

        TTestResult GetTestResultForChild(IDecisionTreeNode child);

        IDecisionTreeLink<TTestResult> GetChildLinkForChild(IDecisionTreeNode child);

        bool TestResultsContains(TTestResult testResult);
    }
}
