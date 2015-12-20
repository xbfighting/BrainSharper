using System;
using System.Collections.Concurrent;
using BrainSharper.Abstract.Algorithms.DecisionTrees.DataStructures;
using BrainSharper.Abstract.Algorithms.DecisionTrees.Helpers;
using BrainSharper.Abstract.Algorithms.DecisionTrees.Processors;
using BrainSharper.Implementations.Algorithms.DecisionTrees.DataStructures.BinaryDecisionTrees;
using BrainSharper.Implementations.Algorithms.DecisionTrees.Processors;

namespace BrainSharper.Implementations.Algorithms.DecisionTrees
{
    public class BinaryDecisionTreeModelBuilder : BaseDecisionTreeModelBuilder
    {
        public BinaryDecisionTreeModelBuilder(
            ISplitQualityChecker splitQualityChecker,
            IBinaryBestSplitSelector binaryBestSplitSelector,
            ILeafBuilder leafBuilder,
            IStatisticalSignificanceChecker statisticalSignificanceChecker = null)
            : base(splitQualityChecker, binaryBestSplitSelector, leafBuilder, statisticalSignificanceChecker)
        {
        }

        protected override IDecisionTreeNode BuildConcreteDecisionTreeNode(ISplittingResult splittingResult,
            ConcurrentDictionary<IDecisionTreeLink, IDecisionTreeNode> children)
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