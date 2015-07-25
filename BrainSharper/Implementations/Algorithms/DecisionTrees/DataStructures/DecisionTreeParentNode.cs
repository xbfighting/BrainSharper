using System.Collections.Generic;
using System.Linq;
using BrainSharper.Abstract.Algorithms.DecisionTrees.DataStructures;

namespace BrainSharper.Implementations.Algorithms.DecisionTrees.DataStructures
{
    public class DecisionTreeParentNode<TTestResult> : IDecisionTreeParentNode<TTestResult>
    {
        public DecisionTreeParentNode(
            bool isLeaf, 
            string decisionFeatureName, 
            IDictionary<IDecisionTreeLink<TTestResult>, IDecisionTreeNode> linksToChildren,
            double trainingDataAccuracy = 0)
        {
            DecisionFeatureName = decisionFeatureName;
            Children = linksToChildren?.Values.ToList() ?? new List<IDecisionTreeNode>();
            LinksToChildren = linksToChildren ?? new Dictionary<IDecisionTreeLink<TTestResult>, IDecisionTreeNode>();
            ChildrenByTestResults = new Dictionary<TTestResult, IDecisionTreeNode>();
            foreach (var childLink in LinksToChildren)
            {
                if (!ChildrenByTestResults.ContainsKey(childLink.Key.TestResult))
                {
                    ChildrenByTestResults.Add(childLink.Key.TestResult, childLink.Value);
                }
            }
            TrainingDataAccuracy = trainingDataAccuracy;
        }

        public double TrainingDataAccuracy { get; }
        public bool IsLeaf => false;
        public string DecisionFeatureName { get; }
        public IList<IDecisionTreeNode> Children { get; }
        public IDictionary<IDecisionTreeLink<TTestResult>, IDecisionTreeNode> LinksToChildren { get; }
        public IDictionary<TTestResult, IDecisionTreeNode> ChildrenByTestResults { get; }
    }
}