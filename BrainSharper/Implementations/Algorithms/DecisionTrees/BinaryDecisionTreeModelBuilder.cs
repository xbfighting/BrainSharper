using System.Collections.Concurrent;
using System.Linq;
using System.Threading.Tasks;
using BrainSharper.Abstract.Algorithms.DecisionTrees;
using BrainSharper.Abstract.Algorithms.DecisionTrees.DataStructures;
using BrainSharper.Abstract.Algorithms.DecisionTrees.DataStructures.BinaryTrees;
using BrainSharper.Abstract.Algorithms.DecisionTrees.Processors;
using BrainSharper.Abstract.Algorithms.Infrastructure;
using BrainSharper.Abstract.Data;
using BrainSharper.General.Utils;
using BrainSharper.Implementations.Algorithms.DecisionTrees.DataStructures.BinaryDecisionTrees;
using BrainSharper.Implementations.Algorithms.DecisionTrees.Processors;

namespace BrainSharper.Implementations.Algorithms.DecisionTrees
{
    public class BinaryDecisionTreeModelBuilder<T> : IDecisionTreeModelBuilder
    {
        private readonly ISplitQualityChecker<bool> _splitQualityChecker;
        private readonly IBinaryBestSplitSelector _binaryBestSplitSelector;
        private readonly ILeafBuilder _leafBuilder; 

        public BinaryDecisionTreeModelBuilder(
            ISplitQualityChecker<bool> splitQualityChecker, 
            IBinaryBestSplitSelector binaryBestSplitSelector,
            ILeafBuilder leafBuilder)
        {
            _splitQualityChecker = splitQualityChecker;
            _binaryBestSplitSelector = binaryBestSplitSelector;
            _leafBuilder = leafBuilder;
        }

        public IPredictionModel BuildModel(IDataFrame dataFrame, string dependentFeatureName, IModelBuilderParams additionalParams)
        {
            if (ShouldStopRecusrsiveBuilding(dataFrame, dependentFeatureName))
            {
                return BuildLeaf(dataFrame, dependentFeatureName);
            }
            return BuildDecisionNode(dataFrame, dependentFeatureName, additionalParams, true);
        }

        public IPredictionModel BuildModel(IDataFrame dataFrame, int dependentFeatureIndex, IModelBuilderParams additionalParams)
        {
            return BuildModel(dataFrame, dataFrame.ColumnNames[dependentFeatureIndex], additionalParams);
        }

        protected virtual IDecisionTreeNode BuildDecisionNode(
            IDataFrame dataFrame, 
            string dependentFeatureName, 
            IModelBuilderParams additionalParams,
            bool isFirstSplit=false)
        {
            if (dataFrame.GetColumnVector<object>(dependentFeatureName).DataItems.Distinct().Count() == 1)
            {
                return BuildLeaf(dataFrame, dependentFeatureName);
            }
            // TODO: later on add additional params indicating which features were already used
            // TODO: handle situation when best split is null - build leaf node immediately
            IBinarySplittingResult splitResult = (IBinarySplittingResult)_binaryBestSplitSelector.SelectBestSplit(
                dataFrame,
                dependentFeatureName,
                _splitQualityChecker);
            var children = new ConcurrentDictionary<IDecisionTreeLink<bool>, IDecisionTreeNode>();
            if (isFirstSplit)
            {
                Parallel.ForEach(splitResult.SplittedDataSets, splitData =>
                {
                    ProcessSplitResult(dataFrame, dependentFeatureName, additionalParams, splitData, children);
                });
            }
            else
            {
                foreach (var splitData in splitResult.SplittedDataSets)
                {
                    ProcessSplitResult(dataFrame, dependentFeatureName, additionalParams, splitData, children);
                }
            }
            return new BinaryDecisionTreeParentNode(
                false, 
                splitResult.SplittingFeatureName,
                children,
                splitResult.SplittingValue,
                splitResult.IsSplitNumeric);
        }

        private void ProcessSplitResult(IDataFrame dataFrame, string dependentFeatureName, IModelBuilderParams additionalParams,
            ISplittedData<bool> splitData, ConcurrentDictionary<IDecisionTreeLink<bool>, IDecisionTreeNode> children)
        {
            if (splitData.SplittedDataFrame.RowCount == 0)
            {
                AddLeafFromSplit(dependentFeatureName, additionalParams, splitData, dataFrame, children);
            }
            else
            {
                AddChildFromSplit(dependentFeatureName, additionalParams, splitData, children);
            }
        }

        private static bool ShouldStopRecusrsiveBuilding(IDataFrame dataFrame, string dependentFeatureName)
        {
            return !dataFrame.GetColumnType(dependentFeatureName).IsNumericType() && dataFrame.GetColumnVector<object>(dependentFeatureName).DataItems.Distinct().Count() == 1;
        }

        private IDecisionTreeLeaf BuildLeaf(IDataFrame dataFrame, string dependentFeatureName)
        {
            return _leafBuilder.BuildLeaf(dataFrame.GetColumnVector(dependentFeatureName), dependentFeatureName);
        }

        private void AddLeafFromSplit(
            string dependentFeatureName,
            IModelBuilderParams additionalParams,
            ISplittedData<bool> splitData,
            IDataFrame baseData,
            ConcurrentDictionary<IDecisionTreeLink<bool>, IDecisionTreeNode> children
            )
        {
            var leafNode = BuildLeaf(baseData, dependentFeatureName);
            var link = (IBinaryDecisionTreeLink)splitData.SplitLink;
            children.TryAdd(link, leafNode);
        }

        private void AddChildFromSplit(
            string dependentFeatureName, 
            IModelBuilderParams additionalParams,
            ISplittedData<bool> splitData, 
            ConcurrentDictionary<IDecisionTreeLink<bool>, IDecisionTreeNode> children)
        {
            var decisionTreeNode = BuildDecisionNode(
                splitData.SplittedDataFrame,
                dependentFeatureName,
                additionalParams);
            var link = (IBinaryDecisionTreeLink)splitData.SplitLink;
            children.TryAdd(link, decisionTreeNode);
        }
    }
}
