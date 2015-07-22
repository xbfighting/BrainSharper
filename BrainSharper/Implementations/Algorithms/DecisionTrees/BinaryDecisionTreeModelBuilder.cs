using System.Collections.Concurrent;
using System.Linq;
using System.Threading.Tasks;
using BrainSharper.Abstract.Algorithms.DecisionTrees;
using BrainSharper.Abstract.Algorithms.DecisionTrees.DataStructures;
using BrainSharper.Abstract.Algorithms.DecisionTrees.DataStructures.BinaryTrees;
using BrainSharper.Abstract.Algorithms.DecisionTrees.Processors;
using BrainSharper.Abstract.Algorithms.Infrastructure;
using BrainSharper.Abstract.Data;
using BrainSharper.Implementations.Algorithms.DecisionTrees.DataStructures.BinaryDecisionTrees;
using BrainSharper.Implementations.Algorithms.DecisionTrees.Processors;

namespace BrainSharper.Implementations.Algorithms.DecisionTrees
{
    public class BinaryDecisionTreeModelBuilder<T> : IDecisionTreeModelBuilder
    {
        private readonly ISplitQualityChecker _splitQualityChecker;
        private readonly IBinaryBestSplitSelector _binaryBestSplitSelector;
        private readonly ILeafBuilder<T> _leafBuilder; 

        public BinaryDecisionTreeModelBuilder(
            ISplitQualityChecker splitQualityChecker, 
            IBinaryBestSplitSelector binaryBestSplitSelector,
            ILeafBuilder<T> leafBuilder)
        {
            _splitQualityChecker = splitQualityChecker;
            _binaryBestSplitSelector = binaryBestSplitSelector;
            _leafBuilder = leafBuilder;
        }

        public IPredictionModel BuildModel(IDataFrame dataFrame, string dependentFeatureName, IModelBuilderParams additionalParams)
        {
            if (dataFrame.GetColumnVector<T>(dependentFeatureName).DataItems.Distinct().Count() == 1)
            {
                return BuildLeaf(dataFrame, dependentFeatureName);
            }
            return BuildDecisionNode(dataFrame, dependentFeatureName, additionalParams, true);
        }

        private IDecisionTreeLeaf<T> BuildLeaf(IDataFrame dataFrame, string dependentFeatureName)
        {
            return _leafBuilder.BuildLeaf(dataFrame.GetColumnVector<T>(dependentFeatureName), dependentFeatureName);
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
            if (dataFrame.GetColumnVector<T>(dependentFeatureName).DataItems.Distinct().Count() == 1)
            {
                return BuildLeaf(dataFrame, dependentFeatureName);
            }
            // TODO: later on add additional params indicating which features were already used
            IBinarySplittingResult splitResult = (IBinarySplittingResult)_binaryBestSplitSelector.SelectBestSplit(
                dataFrame,
                dependentFeatureName,
                _splitQualityChecker);
            var children = new ConcurrentDictionary<IDecisionTreeLink, IDecisionTreeNode>();
            if (isFirstSplit)
            {
                Parallel.ForEach(splitResult.SplittedDataSets, splitData =>
                {
                    AddChildFromSplit(dependentFeatureName, additionalParams, splitData, children);
                });
            }
            else
            {
                foreach (var splitData in splitResult.SplittedDataSets)
                {
                    AddChildFromSplit(dependentFeatureName, additionalParams, splitData, children);
                }
            }
            return new BinaryDecisionTreeParentNode(
                false, 
                splitResult.SplittingFeatureName,
                children,
                splitResult.SplittingValue,
                splitResult.IsSplitNumeric);
        }

        private void AddChildFromSplit(string dependentFeatureName, IModelBuilderParams additionalParams,
           ISplittedData splitData, ConcurrentDictionary<IDecisionTreeLink, IDecisionTreeNode> children)
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
