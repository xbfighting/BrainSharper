using System.Collections.Generic;
using System.Linq;
using BrainSharper.Abstract.Algorithms.DecisionTrees.DataStructures;

namespace BrainSharper.Implementations.Algorithms.DecisionTrees.DataStructures
{
    using System;

    public class DecisionTreeParentNode<TTestResult> : IDecisionTreeParentNode<TTestResult>
    {
        protected IDictionary<IDecisionTreeLink<TTestResult>, IDecisionTreeNode> LinksToChildren { get; }
        protected IDictionary<TTestResult, IDecisionTreeNode> ChildrenByTestResults { get; }
        protected IDictionary<IDecisionTreeNode, IDecisionTreeLink<TTestResult>> LinksByChildren { get; }
        protected IDictionary<IDecisionTreeNode, TTestResult> TestResultsByChildren { get; }

        public DecisionTreeParentNode(
            bool isLeaf, 
            string decisionFeatureName, 
            IDictionary<IDecisionTreeLink<TTestResult>, IDecisionTreeNode> linksToChildren,
            double trainingDataAccuracy = 0)
        {
            this.DecisionFeatureName = decisionFeatureName;
            this.Children = linksToChildren?.Values.ToList() ?? new List<IDecisionTreeNode>();
            this.LinksToChildren = linksToChildren ?? new Dictionary<IDecisionTreeLink<TTestResult>, IDecisionTreeNode>();
            this.ChildrenByTestResults = new Dictionary<TTestResult, IDecisionTreeNode>();
            this.LinksByChildren = new Dictionary<IDecisionTreeNode, IDecisionTreeLink<TTestResult>>();
            this.TestResultsByChildren = new Dictionary<IDecisionTreeNode, TTestResult>();
            this.ChildrenWithTestResults = new List<Tuple<IDecisionTreeLink<TTestResult>, IDecisionTreeNode>>();
            
            foreach (var childLink in this.LinksToChildren)
            {
                this.ChildrenWithTestResults.Add(new Tuple<IDecisionTreeLink<TTestResult>, IDecisionTreeNode>(childLink.Key, childLink.Value));

                if (!this.ChildrenByTestResults.ContainsKey(childLink.Key.TestResult))
                {
                    this.ChildrenByTestResults.Add(childLink.Key.TestResult, childLink.Value);
                }
                if (!this.LinksByChildren.ContainsKey(childLink.Value))
                {
                    this.LinksByChildren.Add(childLink.Value, childLink.Key);
                    this.TestResultsByChildren.Add(childLink.Value, childLink.Key.TestResult);
                }
            }
            this.TrainingDataAccuracy = trainingDataAccuracy;
        }

        public double TrainingDataAccuracy { get; }
        public bool IsLeaf => false;
        public string DecisionFeatureName { get; }
        public IList<IDecisionTreeNode> Children { get; }

        public IList<Tuple<IDecisionTreeLink<TTestResult>, IDecisionTreeNode>> ChildrenWithTestResults { get; }

        public IDecisionTreeNode GetChildForTestResult(TTestResult testResult)
        {
            return this.ChildrenByTestResults[testResult];
        }

        public IDecisionTreeNode GetChildForLink(IDecisionTreeLink<TTestResult> link)
        {
            return this.LinksToChildren[link];
        }

        public TTestResult GetTestResultForChild(IDecisionTreeNode child)
        {
            return this.TestResultsByChildren[child];
        }

        public IDecisionTreeLink<TTestResult> GetChildLinkForChild(IDecisionTreeNode child)
        {
            return this.LinksByChildren[child];
        }

        public bool TestResultsContains(TTestResult testResult)
        {
            return this.ChildrenByTestResults.ContainsKey(testResult);
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
            if (obj.GetType() != this.GetType())
            {
                return false;
            }
            return Equals((DecisionTreeParentNode<TTestResult>)obj);
        }

        protected bool Equals(DecisionTreeParentNode<TTestResult> other)
        {
            var isEqual = this.TrainingDataAccuracy.Equals(other.TrainingDataAccuracy) && string.Equals(this.DecisionFeatureName, other.DecisionFeatureName);
            if (!isEqual)
            {
                return false;
            }
            if (this.Children == null && other.Children == null)
            {
                return true;
            }
            isEqual = this.Children.SequenceEqual(other.Children);
            if (!isEqual)
            {
                return false;
            }
            foreach (var kvp in this.LinksToChildren)
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
                var hashCode = this.TrainingDataAccuracy.GetHashCode();
                hashCode = (hashCode * 397) ^ (this.DecisionFeatureName?.GetHashCode() ?? 0);
                hashCode = (hashCode * 397) ^ (this.Children?.Select(child => child.GetHashCode()).Aggregate(1, (accum, childHash) => accum ^ childHash, val => val) ?? 0);
                hashCode = (hashCode * 397) ^ (this.LinksToChildren?.Select(kvp => kvp.Key.GetHashCode()).Aggregate(1, (accum, linkHsh) => accum ^ linkHsh, val => val) ?? 0);
                return hashCode;
            }
        }
    }
}