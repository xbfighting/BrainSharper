namespace BrainSharper.Implementations.Algorithms.DecisionTrees
{
    using System;
    using System.Collections.Concurrent;
    using System.Linq;
    using System.Threading.Tasks;

    using Abstract.Algorithms.DecisionTrees;
    using Abstract.Algorithms.DecisionTrees.DataStructures;
    using Abstract.Algorithms.DecisionTrees.Processors;
    using Abstract.Algorithms.Infrastructure;
    using Abstract.Data;
    using DataStructures;
    using General.Utils;

    public class MultiSplitDecisionTreeModelBuilder<TDecisionType> : IDecisionTreeModelBuilder
    {
        private readonly ISplitQualityChecker<TDecisionType> splitQualityChecker;
        private readonly IBestSplitSelector<TDecisionType> bestSplitSelector;
        private readonly ILeafBuilder leafBuilder;

        public MultiSplitDecisionTreeModelBuilder(ISplitQualityChecker<TDecisionType> splitQualityChecker, IBestSplitSelector<TDecisionType> bestSplitSelector, ILeafBuilder leafBuilder)
        {
            this.splitQualityChecker = splitQualityChecker;
            this.bestSplitSelector = bestSplitSelector;
            this.leafBuilder = leafBuilder;
        }

        public IPredictionModel BuildModel(IDataFrame dataFrame, string dependentFeatureName, IModelBuilderParams additionalParams)
        {
            if (ShouldStopRecusrsiveBuilding(dataFrame, dependentFeatureName))
            {
                return this.BuildLeaf(dataFrame, dependentFeatureName);
            }
            return this.BuildDecisionNode(dataFrame, dependentFeatureName, additionalParams, false);
        }

        public IPredictionModel BuildModel(IDataFrame dataFrame, int dependentFeatureIndex, IModelBuilderParams additionalParams)
        {
            return this.BuildModel(dataFrame, dataFrame.ColumnNames[dependentFeatureIndex], additionalParams);
        }

        protected virtual IDecisionTreeNode BuildDecisionNode(
            IDataFrame dataFrame, 
            string dependentFeatureName, 
            IModelBuilderParams additionalParams, 
            bool isFirstSplit = false)
        {
            if (dataFrame.GetColumnVector<object>(dependentFeatureName).DataItems.Distinct().Count() == 1)
            {
                return this.BuildLeaf(dataFrame, dependentFeatureName);
            }

            ISplittingResult<TDecisionType> splitResult = this.bestSplitSelector.SelectBestSplit(
                dataFrame, 
                dependentFeatureName, 
                this.splitQualityChecker);
            var children = new ConcurrentDictionary<IDecisionTreeLink<TDecisionType>, IDecisionTreeNode>();
            if (isFirstSplit)
            {
                Parallel.ForEach(
                   splitResult.SplittedDataSets, 
                   splitData =>
                   {
                       this.ProcessSplitResult(dataFrame, dependentFeatureName, additionalParams, splitData, children);
                   });
            }
            else
            {
                foreach (var splitData in splitResult.SplittedDataSets)
                {
                    this.ProcessSplitResult(dataFrame, dependentFeatureName, additionalParams, splitData, children);
                }
            }

            // TODO: later add support for setting numeric split indicators
            return new DecisionTreeParentNode<TDecisionType>(
                false, 
                splitResult.SplittingFeatureName, 
                children);
        }

        private static bool ShouldStopRecusrsiveBuilding(IDataFrame dataFrame, string dependentFeatureName)
        {
            return !dataFrame.GetColumnType(dependentFeatureName).IsNumericType() && dataFrame.GetColumnVector<object>(dependentFeatureName).DataItems.Distinct().Count() == 1;
        }

        private IDecisionTreeLeaf BuildLeaf(IDataFrame dataFrame, string dependentFeatureName)
        {
            return this.leafBuilder.BuildLeaf(dataFrame.GetColumnVector(dependentFeatureName), dependentFeatureName);
        }

        private void ProcessSplitResult(
            IDataFrame dataFrame, 
            string dependentFeatureName, 
            IModelBuilderParams additionalParams, 
            ISplittedData<TDecisionType> splitData, 
            ConcurrentDictionary<IDecisionTreeLink<TDecisionType>, IDecisionTreeNode> children)
        {
            if (splitData.SplittedDataFrame.RowCount == 0)
            {
                this.AddLeafFromSplit(dependentFeatureName, additionalParams, splitData, dataFrame, children);
            }
            else
            {
                this.AddChildFromSplit(dependentFeatureName, additionalParams, splitData, children);
            }
        }

        private void AddLeafFromSplit(
          string dependentFeatureName, 
          IModelBuilderParams additionalParams, 
          ISplittedData<TDecisionType> splitData, 
          IDataFrame baseData, 
          ConcurrentDictionary<IDecisionTreeLink<TDecisionType>, IDecisionTreeNode> children)
        {
            var leafNode = this.BuildLeaf(baseData, dependentFeatureName);
            var link = splitData.SplitLink;
            children.TryAdd(link, leafNode);
        }

        private void AddChildFromSplit(
            string dependentFeatureName, 
            IModelBuilderParams additionalParams, 
            ISplittedData<TDecisionType> splitData, 
            ConcurrentDictionary<IDecisionTreeLink<TDecisionType>, IDecisionTreeNode> children)
        {
            var decisionTreeNode = this.BuildDecisionNode(
                splitData.SplittedDataFrame, 
                dependentFeatureName, 
                additionalParams);
            var link = splitData.SplitLink;
            var exists = children.ContainsKey(link);
            children.TryAdd(link, decisionTreeNode);
        }

    }
}
