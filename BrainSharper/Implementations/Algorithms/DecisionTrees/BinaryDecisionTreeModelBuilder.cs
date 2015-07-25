namespace BrainSharper.Implementations.Algorithms.DecisionTrees
{
    using System.Collections.Concurrent;
    using System.Linq;
    using System.Threading.Tasks;

    using Abstract.Algorithms.DecisionTrees;
    using Abstract.Algorithms.DecisionTrees.DataStructures;
    using Abstract.Algorithms.DecisionTrees.DataStructures.BinaryTrees;
    using Abstract.Algorithms.DecisionTrees.Processors;
    using Abstract.Algorithms.Infrastructure;
    using Abstract.Data;

    using DataStructures.BinaryDecisionTrees;
    using General.Utils;
    using Processors;

    public class BinaryDecisionTreeModelBuilder<T> : IDecisionTreeModelBuilder
    {
        private readonly ISplitQualityChecker<bool> splitQualityChecker;
        private readonly IBinaryBestSplitSelector binaryBestSplitSelector;
        private readonly ILeafBuilder leafBuilder; 

        public BinaryDecisionTreeModelBuilder(
            ISplitQualityChecker<bool> splitQualityChecker, 
            IBinaryBestSplitSelector binaryBestSplitSelector,
            ILeafBuilder leafBuilder)
        {
            this.splitQualityChecker = splitQualityChecker;
            this.binaryBestSplitSelector = binaryBestSplitSelector;
            this.leafBuilder = leafBuilder;
        }

        public IPredictionModel BuildModel(IDataFrame dataFrame, string dependentFeatureName, IModelBuilderParams additionalParams)
        {
            if (ShouldStopRecusrsiveBuilding(dataFrame, dependentFeatureName))
            {
                return this.BuildLeaf(dataFrame, dependentFeatureName);
            }
            return this.BuildDecisionNode(dataFrame, dependentFeatureName, additionalParams, true);
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

            // TODO: later on add additional params indicating which features were already used
            IBinarySplittingResult splitResult = (IBinarySplittingResult)this.binaryBestSplitSelector.SelectBestSplit(
                dataFrame,
                dependentFeatureName,
                this.splitQualityChecker);
            var children = new ConcurrentDictionary<IDecisionTreeLink<bool>, IDecisionTreeNode>();
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
            return new BinaryDecisionTreeParentNode(
                false, 
                splitResult.SplittingFeatureName,
                children,
                splitResult.SplittingValue,
                splitResult.IsSplitNumeric);
        }

        private static bool ShouldStopRecusrsiveBuilding(IDataFrame dataFrame, string dependentFeatureName)
        {
            return !dataFrame.GetColumnType(dependentFeatureName).IsNumericType() && dataFrame.GetColumnVector<object>(dependentFeatureName).DataItems.Distinct().Count() == 1;
        }

        private void ProcessSplitResult(
            IDataFrame dataFrame,
            string dependentFeatureName,
            IModelBuilderParams additionalParams,
            ISplittedData<bool> splitData,
            ConcurrentDictionary<IDecisionTreeLink<bool>, IDecisionTreeNode> children)
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

        private IDecisionTreeLeaf BuildLeaf(IDataFrame dataFrame, string dependentFeatureName)
        {
            return this.leafBuilder.BuildLeaf(dataFrame.GetColumnVector(dependentFeatureName), dependentFeatureName);
        }

        private void AddLeafFromSplit(
            string dependentFeatureName,
            IModelBuilderParams additionalParams,
            ISplittedData<bool> splitData,
            IDataFrame baseData,
            ConcurrentDictionary<IDecisionTreeLink<bool>, IDecisionTreeNode> children)
        {
            var leafNode = this.BuildLeaf(baseData, dependentFeatureName);
            var link = (IBinaryDecisionTreeLink)splitData.SplitLink;
            children.TryAdd(link, leafNode);
        }

        private void AddChildFromSplit(
            string dependentFeatureName, 
            IModelBuilderParams additionalParams,
            ISplittedData<bool> splitData, 
            ConcurrentDictionary<IDecisionTreeLink<bool>, IDecisionTreeNode> children)
        {
            var decisionTreeNode = this.BuildDecisionNode(
                splitData.SplittedDataFrame,
                dependentFeatureName,
                additionalParams);
            var link = (IBinaryDecisionTreeLink)splitData.SplitLink;
            children.TryAdd(link, decisionTreeNode);
        }
    }
}
