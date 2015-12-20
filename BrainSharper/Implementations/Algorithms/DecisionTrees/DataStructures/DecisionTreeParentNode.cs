using System;
using System.Collections.Generic;
using System.Linq;
using BrainSharper.Abstract.Algorithms.DecisionTrees.DataStructures;

namespace BrainSharper.Implementations.Algorithms.DecisionTrees.DataStructures
{
    #region

    

    #endregion

    public class DecisionTreeParentNode : IDecisionTreeParentNode
    {
        public DecisionTreeParentNode(
            bool isLeaf,
            string decisionFeatureName,
            IDictionary<IDecisionTreeLink, IDecisionTreeNode> linksToChildren,
            double trainingDataAccuracy = 0)
        {
            DecisionFeatureName = decisionFeatureName;
            Children = linksToChildren?.Values.ToList() ?? new List<IDecisionTreeNode>();
            LinksToChildren = linksToChildren ?? new Dictionary<IDecisionTreeLink, IDecisionTreeNode>();
            ChildrenByTestResults = new Dictionary<object, IDecisionTreeNode>();
            LinksByChildren = new Dictionary<IDecisionTreeNode, IDecisionTreeLink>();
            TestResultsByChildren = new Dictionary<IDecisionTreeNode, object>();
            ChildrenWithTestResults = new List<Tuple<IDecisionTreeLink, IDecisionTreeNode>>();

            foreach (var childLink in LinksToChildren)
            {
                ChildrenWithTestResults.Add(
                    new Tuple<IDecisionTreeLink, IDecisionTreeNode>(childLink.Key, childLink.Value));

                if (!ChildrenByTestResults.ContainsKey(childLink.Key.TestResult))
                {
                    ChildrenByTestResults.Add(childLink.Key.TestResult, childLink.Value);
                }

                if (!LinksByChildren.ContainsKey(childLink.Value))
                {
                    LinksByChildren.Add(childLink.Value, childLink.Key);
                    TestResultsByChildren.Add(childLink.Value, childLink.Key.TestResult);
                }
            }

            TrainingDataAccuracy = trainingDataAccuracy;
        }

        protected IDictionary<IDecisionTreeLink, IDecisionTreeNode> LinksToChildren { get; }
        protected IDictionary<object, IDecisionTreeNode> ChildrenByTestResults { get; }
        protected IDictionary<IDecisionTreeNode, IDecisionTreeLink> LinksByChildren { get; }
        protected IDictionary<IDecisionTreeNode, object> TestResultsByChildren { get; }
        public double TrainingDataAccuracy { get; }
        public bool IsLeaf => false;
        public string DecisionFeatureName { get; }
        public IList<IDecisionTreeNode> Children { get; }
        public IList<Tuple<IDecisionTreeLink, IDecisionTreeNode>> ChildrenWithTestResults { get; }

        public IDecisionTreeNode GetChildForTestResult(object testResult)
        {
            return ChildrenByTestResults[testResult];
        }

        public IDecisionTreeNode GetChildForLink(IDecisionTreeLink link)
        {
            return LinksToChildren[link];
        }

        public object GetTestResultForChild(IDecisionTreeNode child)
        {
            return TestResultsByChildren[child];
        }

        public IDecisionTreeLink GetChildLinkForChild(IDecisionTreeNode child)
        {
            return LinksByChildren[child];
        }

        public bool TestResultsContains(object testResult)
        {
            return ChildrenByTestResults.ContainsKey(testResult);
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj))
            {
                return false;
            }

            if (ReferenceEquals(this, obj))
            {
                return true;
            }

            if (obj.GetType() != GetType())
            {
                return false;
            }

            return Equals((DecisionTreeParentNode) obj);
        }

        protected bool Equals(DecisionTreeParentNode other)
        {
            var isEqual = TrainingDataAccuracy.Equals(other.TrainingDataAccuracy)
                          && string.Equals(DecisionFeatureName, other.DecisionFeatureName);
            if (!isEqual)
            {
                return false;
            }

            if (Children == null && other.Children == null)
            {
                return true;
            }

            isEqual = Children.SequenceEqual(other.Children);
            if (!isEqual)
            {
                return false;
            }

            foreach (var kvp in LinksToChildren)
            {
                if (other.LinksToChildren.ContainsKey(kvp.Key))
                {
                    return false;
                }
            }

            return true;
        }

        public override int GetHashCode()
        {
            unchecked
            {
                var hashCode = TrainingDataAccuracy.GetHashCode();
                hashCode = (hashCode*397) ^ (DecisionFeatureName?.GetHashCode() ?? 0);
                hashCode = (hashCode*397)
                           ^ (Children?.Select(child => child.GetHashCode())
                               .Aggregate(1, (accum, childHash) => accum ^ childHash, val => val) ?? 0);
                hashCode = (hashCode*397)
                           ^ (LinksToChildren?.Select(kvp => kvp.Key.GetHashCode())
                               .Aggregate(1, (accum, linkHsh) => accum ^ linkHsh, val => val) ?? 0);
                return hashCode;
            }
        }
    }
}