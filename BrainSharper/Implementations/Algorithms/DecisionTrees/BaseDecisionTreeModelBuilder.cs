namespace BrainSharper.Implementations.Algorithms.DecisionTrees
{
    using System.Collections.Concurrent;
    using System.Linq;
    using System.Threading.Tasks;

    using Abstract.Algorithms.DecisionTrees;
    using Abstract.Algorithms.DecisionTrees.DataStructures;
    using Abstract.Algorithms.DecisionTrees.Processors;
    using Abstract.Algorithms.Infrastructure;
    using Abstract.Data;

    using General.Utils;

    public abstract class BaseDecisionTreeModelBuilder : IDecisionTreeModelBuilder
    {
        protected readonly ISplitQualityChecker SplitQualityChecker;
        protected readonly IBestSplitSelector BestSplitSelector;
        protected readonly ILeafBuilder LeafBuilder;

        protected BaseDecisionTreeModelBuilder(ISplitQualityChecker splitQualityChecker, IBestSplitSelector bestSplitSelector, ILeafBuilder leafBuilder)
        {
            SplitQualityChecker = splitQualityChecker;
            BestSplitSelector = bestSplitSelector;
            LeafBuilder = leafBuilder;
        }

        public IPredictionModel BuildModel(
            IDataFrame dataFrame,
            string dependentFeatureName,
            IModelBuilderParams additionalParams)
        {
            if (ShouldStopRecusrsiveBuilding(dataFrame, dependentFeatureName))
            {
                return this.BuildLeaf(dataFrame, dependentFeatureName);
            }
            return this.BuildDecisionNode(dataFrame, dependentFeatureName, additionalParams, true);
        }

        public IPredictionModel BuildModel(
            IDataFrame dataFrame,
            int dependentFeatureIndex,
            IModelBuilderParams additionalParams)
        {
            return this.BuildModel(dataFrame, dataFrame.ColumnNames[dependentFeatureIndex], additionalParams);
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
          IModelBuilderParams additionalParams,
          bool isFirstSplit = false)
        {
            if (dataFrame.GetColumnVector<object>(dependentFeatureName).DataItems.Distinct().Count() == 1)
            {
                return this.BuildLeaf(dataFrame, dependentFeatureName);
            }

            // TODO: later on add additional params indicating which features were already used
            ISplittingResult splitResult = BestSplitSelector.SelectBestSplit(
                dataFrame,
                dependentFeatureName,
                SplitQualityChecker);
            var children = new ConcurrentDictionary<IDecisionTreeLink, IDecisionTreeNode>();
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
            return BuildConcreteDecisionTreeNode(splitResult, children);
        }

        protected void ProcessSplitResult(
          IDataFrame dataFrame,
          string dependentFeatureName,
          IModelBuilderParams additionalParams,
          ISplittedData splitData,
          ConcurrentDictionary<IDecisionTreeLink, IDecisionTreeNode> children)
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

        protected virtual void AddChildFromSplit(
           string dependentFeatureName,
           IModelBuilderParams additionalParams,
           ISplittedData splitData,
           ConcurrentDictionary<IDecisionTreeLink, IDecisionTreeNode> children)
        {
            var decisionTreeNode = BuildDecisionNode(
                splitData.SplittedDataFrame,
                dependentFeatureName,
                additionalParams);
            var link = splitData.SplitLink;
            children.TryAdd(link, decisionTreeNode);
        }

        protected virtual void AddLeafFromSplit(
           string dependentFeatureName,
           IModelBuilderParams additionalParams,
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
            return LeafBuilder.BuildLeaf(dataFrame.GetColumnVector(dependentFeatureName), dependentFeatureName);
        }
    }
}
