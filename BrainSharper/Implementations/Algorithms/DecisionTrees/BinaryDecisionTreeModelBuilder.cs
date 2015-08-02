﻿namespace BrainSharper.Implementations.Algorithms.DecisionTrees
{
    using System;
    using System.Collections.Concurrent;

    using Abstract.Algorithms.DecisionTrees.DataStructures;
    using Abstract.Algorithms.DecisionTrees.Processors;
    using DataStructures.BinaryDecisionTrees;
    using Processors;

    public class BinaryDecisionTreeModelBuilder : BaseDecisionTreeModelBuilder
    {
        public BinaryDecisionTreeModelBuilder(
            ISplitQualityChecker splitQualityChecker, 
            IBinaryBestSplitSelector binaryBestSplitSelector, 
            ILeafBuilder leafBuilder)
            : base(splitQualityChecker, binaryBestSplitSelector, leafBuilder)
        {
        }

        protected override IDecisionTreeNode BuildConcreteDecisionTreeNode(ISplittingResult splittingResult, ConcurrentDictionary<IDecisionTreeLink, IDecisionTreeNode> children)
        {
            var binarySplittingResults = splittingResult as IBinarySplittingResult;
            if (binarySplittingResults == null)
            {
                throw new ArgumentException("Invalid split results passed to binary decision tree builder");
            }

            return new BinaryDecisionTreeParentNode(
                false, 
                splittingResult.SplittingFeatureName, 
                children, 
                binarySplittingResults.SplittingValue, 
                binarySplittingResults.IsSplitNumeric);
        }
    }
}