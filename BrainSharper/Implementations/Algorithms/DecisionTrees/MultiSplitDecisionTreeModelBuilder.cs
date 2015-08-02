namespace BrainSharper.Implementations.Algorithms.DecisionTrees
{
    using System.Collections.Concurrent;

    using Abstract.Algorithms.DecisionTrees.DataStructures;
    using Abstract.Algorithms.DecisionTrees.Processors;

    using DataStructures;
    using DataStructures.BinaryDecisionTrees;

    public class MultiSplitDecisionTreeModelBuilder : BaseDecisionTreeModelBuilder
    {
        public MultiSplitDecisionTreeModelBuilder(
            ISplitQualityChecker splitQualityChecker,
            IBestSplitSelector bestSplitSelector,
            ILeafBuilder leafBuilder)
            : base(splitQualityChecker, bestSplitSelector, leafBuilder)
        {
        }

        protected override IDecisionTreeNode BuildConcreteDecisionTreeNode(ISplittingResult splittingResult, ConcurrentDictionary<IDecisionTreeLink, IDecisionTreeNode> children)
        {
            if (splittingResult is IBinarySplittingResult)
            {
                var binarySplitResult = (IBinarySplittingResult)splittingResult;
                return new BinaryDecisionTreeParentNode(
                    false,
                    splittingResult.SplittingFeatureName,
                    children,
                    binarySplitResult.SplittingValue,
                    binarySplitResult.IsSplitNumeric);
            }
            return new DecisionTreeParentNode(
                false,
                splittingResult.SplittingFeatureName,
                children);
        }
    }
}
