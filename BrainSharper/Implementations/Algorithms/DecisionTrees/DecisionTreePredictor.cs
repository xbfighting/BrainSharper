using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using BrainSharper.Abstract.Algorithms.DecisionTrees.DataStructures;
using BrainSharper.Abstract.Algorithms.DecisionTrees.DataStructures.BinaryTrees;
using BrainSharper.Abstract.Algorithms.Infrastructure;
using BrainSharper.Abstract.Data;

namespace BrainSharper.Implementations.Algorithms.DecisionTrees
{
    public class DecisionTreePredictor<TTestResult> : IPredictor<object>
    {
        public IList<object> Predict(IDataFrame queryDataFrame, IPredictionModel model, int dependentFeatureIndex)
        {
            return Predict(queryDataFrame, model, queryDataFrame.ColumnNames[dependentFeatureIndex]);
        }

        public IList<object> Predict(IDataFrame queryDataFrame, IPredictionModel model, string dependentFeatureName)
        {
            if (!(model is IDecisionTreeNode))
            {
                throw new ArgumentException("Invalid model passed to Decision Tree Predictor");
            }
            var results = new ConcurrentBag<Tuple<int, object>>();
            for (int rowIdx = 0; rowIdx < queryDataFrame.RowCount; rowIdx++)
            {
                IDataVector<object> dataVector = queryDataFrame.GetRowVector(rowIdx);
                object predictionResults = ProcessInstance(dataVector, (IDecisionTreeNode)model);
                results.Add(new Tuple<int, object>(rowIdx, predictionResults));
            }
            return results.OrderBy(tpl => tpl.Item1).Select(tpl => tpl.Item2).ToList();
        }

        private object ProcessInstance(IDataVector<object> vector, IDecisionTreeNode decisionTree)
        {
            if (decisionTree is IDecisionTreeLeaf)
            {
                if (decisionTree is IDecisionTreeRegressionAndModelLeaf)
                {
                    // TODO: handle regression/modelling case here
                    var regressionLeaf = decisionTree as IDecisionTreeRegressionAndModelLeaf;
                    var numericVector = vector.NumericVector;
                    return null;
                }
                var classificationLeaf = decisionTree as IDecisionTreeLeaf;
                return classificationLeaf.LeafValue;
            }
            else
            {
                var parentNode = decisionTree as IDecisionTreeParentNode<TTestResult>;
                if (parentNode is IBinaryDecisionTreeParentNode)
                {
                    return ProcessBinarySplit(vector, parentNode as IBinaryDecisionTreeParentNode);
                }
                else
                {
                    // TODO: handle multi split here
                }
            }
            return null;
        }

        private object ProcessBinarySplit(
            IDataVector<object> vector,
            IBinaryDecisionTreeParentNode binaryDecisionTreeNode)
        {
            string decisionFeature = binaryDecisionTreeNode.DecisionFeatureName;
            if (!vector.FeatureNames.Contains(decisionFeature))
            {
                throw new ArgumentException($"Invalid vector passed for prediction. Unknown feature {decisionFeature}");
            }
            object vectorValue = vector[decisionFeature];
            object decisionValue = binaryDecisionTreeNode.DecisionValue;
            if (binaryDecisionTreeNode.IsValueNumeric)
            {
                var isLower = Convert.ToDouble(vectorValue) < Convert.ToDouble(decisionValue);
                var childToFollow = isLower ? binaryDecisionTreeNode.LeftChild : binaryDecisionTreeNode.RightChild;
                return ProcessInstance(vector, childToFollow);
            }
            else
            {
                var isEqual = vectorValue.Equals(decisionValue);
                var childToFollow = isEqual ? binaryDecisionTreeNode.LeftChild : binaryDecisionTreeNode.RightChild;
                return ProcessInstance(vector, childToFollow);
            }
        }
    }
}
