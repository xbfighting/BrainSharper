namespace BrainSharper.Implementations.Algorithms.DecisionTrees
{
    using System;
    using System.Collections.Concurrent;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;

    using Abstract.Algorithms.DecisionTrees;
    using Abstract.Algorithms.DecisionTrees.DataStructures;
    using Abstract.Algorithms.DecisionTrees.Processors;
    using Abstract.Algorithms.Infrastructure;
    using Abstract.Data;

    using BrainSharper.Abstract.Algorithms.DecisionTrees.Helpers;

    using Processors;

    using General.Utils;

    public abstract class BaseDecisionTreeModelBuilder : IDecisionTreeModelBuilder
    {
        protected readonly ISplitQualityChecker SplitQualityChecker;
        protected readonly IBestSplitSelector BestSplitSelector;
        protected readonly ILeafBuilder LeafBuilder;
        private readonly IStatisticalSignificanceChecker StatisticalSignificanceChecker;

        protected BaseDecisionTreeModelBuilder(
            ISplitQualityChecker splitQualityChecker, 
            IBestSplitSelector bestSplitSelector, 
            ILeafBuilder leafBuilder, 
            IStatisticalSignificanceChecker statisticalSignificanceChecker = null)
        {
            SplitQualityChecker = splitQualityChecker;
            BestSplitSelector = bestSplitSelector;
            LeafBuilder = leafBuilder;
            this.StatisticalSignificanceChecker = statisticalSignificanceChecker;
        }

        public IPredictionModel BuildModel(
            IDataFrame dataFrame,
            string dependentFeatureName,
            IModelBuilderParams additionalParams)
        {
            if (!(additionalParams is IDecisionTreeModelBuilderParams))
            {
                throw new ArgumentException("Invalid params passed for Decision Tree Model Builder!");
            }
            if (ShouldStopRecusrsiveBuilding(dataFrame, dependentFeatureName))
            {
                return BuildLeaf(dataFrame, dependentFeatureName);
            }

            var decisionTreeParams = (IDecisionTreeModelBuilderParams)additionalParams;
            var useParallelProcessing = decisionTreeParams.ProcessSubtreesCreationInParallel;
            var alreadyUsedAttributesInfo = new AlreadyUsedAttributesInfo();
            //TODO: reduce the number of parameters - maybe some nicer DTO?
            var node = this.BuildDecisionNode(dataFrame, dependentFeatureName, decisionTreeParams, alreadyUsedAttributesInfo, 0, useParallelProcessing);
            return node;
        }

        public IPredictionModel BuildModel(
            IDataFrame dataFrame,
            int dependentFeatureIndex,
            IModelBuilderParams additionalParams)
        {
            return BuildModel(dataFrame, dataFrame.ColumnNames[dependentFeatureIndex], additionalParams);
        }

        protected static bool ShouldStopRecusrsiveBuilding(IDataFrame dataFrame, string dependentFeatureName)
        {
            return !dataFrame.GetColumnType(dependentFeatureName).IsNumericType() && dataFrame.GetColumnVector<object>(dependentFeatureName).DataItems.Distinct().Count() == 1;
        }

        protected abstract IDecisionTreeNode BuildConcreteDecisionTreeNode(
            ISplittingResult splittingResult, 
            ConcurrentDictionary<IDecisionTreeLink, IDecisionTreeNode> children);

        protected virtual IDecisionTreeNode BuildDecisionNode(
            IDataFrame dataFrame, 
            string dependentFeatureName,
            IDecisionTreeModelBuilderParams additionalParams,
            IAlredyUsedAttributesInfo alreadyUsedAttributesInfo, 
            int treeDepth,
            bool isFirstSplit = false)
        {
            if (dataFrame.GetColumnVector<object>(dependentFeatureName).DataItems.Distinct().Count() == 1 || MaximalTreeDepthHasBeenReached(additionalParams, treeDepth))
            {
                return BuildLeaf(dataFrame, dependentFeatureName);
            }

            // TODO: later on add additional params indicating which features were already used
            ISplittingResult splitResult = BestSplitSelector.SelectBestSplit(
                dataFrame,
                dependentFeatureName,
                SplitQualityChecker,
                alreadyUsedAttributesInfo);
            if (SplitIsEmpty(splitResult))
            {
                return BuildLeaf(dataFrame, dependentFeatureName);
            }

            if (additionalParams.UsePrunningHeuristicDuringTreeBuild && this.StatisticalSignificanceChecker != null)
            {
                var isSplitSignificant = StatisticalSignificanceChecker.IsSplitStatisticallySignificant(
                    dataFrame,
                    splitResult,
                    dependentFeatureName);
                if (!isSplitSignificant)
                {
                    return BuildLeaf(dataFrame, dependentFeatureName);
                }
            }

            var children = new ConcurrentDictionary<IDecisionTreeLink, IDecisionTreeNode>();
            if (isFirstSplit)
            {
                Parallel.ForEach(
                    splitResult.SplittedDataSets,
                    splitData =>
                    {
                        this.AddChildFromSplit(dependentFeatureName, additionalParams, splitData, children, alreadyUsedAttributesInfo, treeDepth + 1);
                    });
            }
            else
            {
                foreach (var splitData in splitResult.SplittedDataSets)
                {
                    this.AddChildFromSplit(dependentFeatureName, additionalParams, splitData, children, alreadyUsedAttributesInfo, treeDepth + 1);
                }
            }
            return BuildConcreteDecisionTreeNode(splitResult, children);
        }

        protected virtual void AddChildFromSplit(
            string dependentFeatureName, 
            IDecisionTreeModelBuilderParams additionalParams, 
            ISplittedData splitData, 
            ConcurrentDictionary<IDecisionTreeLink, IDecisionTreeNode> children, 
            IAlredyUsedAttributesInfo alreadyUsedAttributesInfo,
            int treeDepth)
        {
            var decisionTreeNode = BuildDecisionNode(
                splitData.SplittedDataFrame,
                dependentFeatureName,
                additionalParams,
                alreadyUsedAttributesInfo,
                treeDepth);
            var link = splitData.SplitLink;
            children.TryAdd(link, decisionTreeNode);
        }

        protected virtual void AddLeafFromSplit(
            string dependentFeatureName, 
            IDecisionTreeModelBuilderParams additionalParams, 
            ISplittedData splitData, 
            IDataFrame baseData, 
            ConcurrentDictionary<IDecisionTreeLink, IDecisionTreeNode> children)
        {
            var leafNode = BuildLeaf(baseData, dependentFeatureName);
            var link = splitData.SplitLink;
            children.TryAdd(link, leafNode);
        }

        protected IDecisionTreeLeaf BuildLeaf(IDataFrame dataFrame, string dependentFeatureName)
        {
            return LeafBuilder.BuildLeaf(dataFrame, dependentFeatureName);
        }

        //TODO: AAA!!! Implement this in split objects!!!
        private static bool SplitIsEmpty(ISplittingResult splitResult)
        {
            return splitResult == null
                   || splitResult.SplittedDataSets.Any(
                       splitSet => splitSet?.SplittedDataFrame == null || splitSet.SplittedDataFrame.RowCount == 0);
        }

        private static bool MaximalTreeDepthHasBeenReached(IDecisionTreeModelBuilderParams additionalParams, int treeDepth)
        {
            return additionalParams.MaximalTreeDepth.HasValue && treeDepth >= additionalParams.MaximalTreeDepth;
        }
    }
}
