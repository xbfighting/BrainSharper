﻿using System.Collections.Generic;
using System.Linq;
using BrainSharper.Abstract.Algorithms.DecisionTrees;
using BrainSharper.Abstract.Algorithms.DecisionTrees.DataStructures;
using BrainSharper.Abstract.Algorithms.DecisionTrees.DataStructures.BinaryTrees;

namespace BrainSharper.Implementations.Algorithms.DecisionTrees.BinaryDecisionTrees
{
    public class BinaryDecisionTreeParentNode<TDecisionValue> : DecisionTreeParentNode, IBinaryDecisionTreeParentNode<TDecisionValue>
    {
        public BinaryDecisionTreeParentNode(
            bool isLeaf, 
            string decisionFeatureName, 
            IDictionary<IDecisionTreeLink, IDecisionTreeNode> linksToChildren, TDecisionValue decisionValue) : base(isLeaf, decisionFeatureName, linksToChildren)
        {
            DecisionValue = decisionValue;
            TestResultsWithChildren = linksToChildren.ToDictionary(kvp => kvp.Key as IBinaryDecisionTreeLink, kvp => kvp.Value);
            foreach (var link in linksToChildren)
            {
                var binaryTreeLink = link.Key as IBinaryDecisionTreeLink;
                if (binaryTreeLink != null)
                {
                    if (!binaryTreeLink.TestValue)
                    {
                        LeftChild = link.Value;
                        LeftChildLink = binaryTreeLink;
                    }
                    else
                    {
                        RightChild = link.Value;
                        RightChildLink = binaryTreeLink;
                    }
                }
            }
        }

        public IDecisionTreeNode LeftChild { get; }
        public IBinaryDecisionTreeLink LeftChildLink { get; }

        public IDecisionTreeNode RightChild { get; }
        public IBinaryDecisionTreeLink RightChildLink { get; }

        public TDecisionValue DecisionValue { get; }
        public IDictionary<IBinaryDecisionTreeLink, IDecisionTreeNode> TestResultsWithChildren { get; }
    }
}