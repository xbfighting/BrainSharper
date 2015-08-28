namespace BrainSharper.Implementations.Algorithms.DecisionTrees
{
    using System;
    using System.Collections.Concurrent;
    using System.Collections.Generic;
    using System.Linq;
    using Abstract.Algorithms.DecisionTrees.DataStructures;
    using Abstract.Algorithms.DecisionTrees.DataStructures.BinaryTrees;
    using Abstract.Algorithms.Infrastructure;
    using Abstract.Data;

    using MathNet.Numerics.LinearAlgebra;

    public class DecisionTreePredictor<TDecisionValue> : IPredictor<TDecisionValue>
    {
        public IList<TDecisionValue> Predict(IDataFrame queryDataFrame, IPredictionModel model, int dependentFeatureIndex)
        {
            return this.Predict(queryDataFrame, model, queryDataFrame.ColumnNames[dependentFeatureIndex]);
        }

        public IList<TDecisionValue> Predict(IDataFrame queryDataFrame, IPredictionModel model, string dependentFeatureName)
        {
            if (!(model is IDecisionTreeNode))
            {
                throw new ArgumentException("Invalid model passed to Decision Tree Predictor");
            }
            var results = new ConcurrentBag<Tuple<int, TDecisionValue>>();
            var queryDataFrameWithoutDependentFeature =
                queryDataFrame.GetSubsetByColumns(
                    queryDataFrame.ColumnNames.Except(new[] { dependentFeatureName }).ToList());
            for (int rowIdx = 0; rowIdx < queryDataFrameWithoutDependentFeature.RowCount; rowIdx++)
            {
                IDataVector<TDecisionValue> dataVector = queryDataFrameWithoutDependentFeature.GetRowVector<TDecisionValue>(rowIdx);
                Tuple<TDecisionValue, double> predictionResults = ProcessInstance(dataVector, (IDecisionTreeNode)model, 1.0);
                results.Add(new Tuple<int, TDecisionValue>(rowIdx, predictionResults.Item1));
            }
            return results.OrderBy(tpl => tpl.Item1).Select(tpl => tpl.Item2).ToList();
        }

        private Tuple<TDecisionValue, double> ProcessInstance(IDataVector<TDecisionValue> vector, IDecisionTreeNode decisionTree, double probabilitiesProductSoFar)
        {
            if (decisionTree is IDecisionTreeLeaf)
            {
                return HandleLeaf(vector, decisionTree, probabilitiesProductSoFar);
            }
            var parentNode = decisionTree as IDecisionTreeParentNode;
            if (parentNode is IBinaryDecisionTreeParentNode)
            {
                return this.ProcessBinarySplit(vector, parentNode as IBinaryDecisionTreeParentNode, probabilitiesProductSoFar);
            }
            return this.ProcessMultiValueSplit(vector, parentNode, probabilitiesProductSoFar);
        }

        private static Tuple<TDecisionValue, double> HandleLeaf(IDataVector<TDecisionValue> vector, IDecisionTreeNode decisionTree, double probabilitiesProductSoFar)
        {
            if (decisionTree is IDecisionTreeRegressionAndModelLeaf)
            {
                return HandleRegressionAndModelLeaf(vector, decisionTree, probabilitiesProductSoFar);
            }
            var classificationLeaf = decisionTree as IDecisionTreeLeaf;
            return new Tuple<TDecisionValue, double>((TDecisionValue)classificationLeaf.LeafValue, probabilitiesProductSoFar);
        }

        private static Tuple<TDecisionValue, double> HandleRegressionAndModelLeaf(
            IDataVector<TDecisionValue> vector,
            IDecisionTreeNode decisionTree,
            double probabilitiesProductSoFar)
        {
            var regressionLeaf = decisionTree as IDecisionTreeRegressionAndModelLeaf;
            var numericVector = vector.NumericVector.ToList();
            numericVector.Insert(0, 1.0);
            var vectorWithIntercept = Vector<double>.Build.DenseOfArray(numericVector.ToArray());
            double predictedVal = 0.0;
            if (regressionLeaf.ModelWeights != null)
            {
                predictedVal =
                    vectorWithIntercept.DotProduct(Vector<double>.Build.DenseOfArray(regressionLeaf.ModelWeights.ToArray()));
            }
            else
            {
                predictedVal = regressionLeaf.DecisionMeanValue;
            }
            return new Tuple<TDecisionValue, double>(
                (TDecisionValue)Convert.ChangeType(predictedVal, typeof(TDecisionValue)),
                probabilitiesProductSoFar);
        }

        private Tuple<TDecisionValue, double> ProcessBinarySplit(
            IDataVector<TDecisionValue> vector,
            IBinaryDecisionTreeParentNode binaryDecisionTreeNode,
            double probabilitiesProductSoFar)
        {
            string decisionFeature = binaryDecisionTreeNode.DecisionFeatureName;
            if (!vector.FeatureNames.Contains(decisionFeature))
            {
                throw new ArgumentException($"Invalid vector passed for prediction. Unknown feature {decisionFeature}");
            }
            TDecisionValue vectorValue = vector[decisionFeature];
            TDecisionValue decisionValue = (TDecisionValue)binaryDecisionTreeNode.DecisionValue;
            if (binaryDecisionTreeNode.IsValueNumeric)
            {
                var isLower = Convert.ToDouble(vectorValue) < Convert.ToDouble(decisionValue);
                var childToFollow = isLower ? binaryDecisionTreeNode.LeftChild : binaryDecisionTreeNode.RightChild;
                return this.ProcessInstance(vector, childToFollow, probabilitiesProductSoFar * binaryDecisionTreeNode.RightChildLink.InstancesPercentage);
            }
            else
            {
                var isEqual = vectorValue.Equals(decisionValue);
                var childToFollow = isEqual ? binaryDecisionTreeNode.RightChild : binaryDecisionTreeNode.LeftChild;
                return this.ProcessInstance(vector, childToFollow, probabilitiesProductSoFar * binaryDecisionTreeNode.LeftChildLink.InstancesPercentage);
            }
        }

        private Tuple<TDecisionValue, double> ProcessMultiValueSplit(
            IDataVector<TDecisionValue> vector,
            IDecisionTreeParentNode multiValueDecisionTreeNode,
            double probabilitiesProductSoFar)
        {
            string decisionFeature = multiValueDecisionTreeNode.DecisionFeatureName;
            if (!vector.FeatureNames.Contains(decisionFeature))
            {
                throw new ArgumentException($"Invalid vector passed for prediction. Unknown feature {decisionFeature}");
            }
            TDecisionValue vectorValue = vector[decisionFeature];
            if (multiValueDecisionTreeNode.TestResultsContains(vectorValue))
            {
                // TODO: optimize for a single query (maybe?) - return Tuple
                var childToFollow = multiValueDecisionTreeNode.GetChildForTestResult(vectorValue);
                var linkToChild = multiValueDecisionTreeNode.GetChildLinkForChild(childToFollow);
                return ProcessInstance(vector, childToFollow, probabilitiesProductSoFar * linkToChild.InstancesPercentage);
            }

            var results = new Dictionary<TDecisionValue, double>();
            foreach (var child in multiValueDecisionTreeNode.ChildrenWithTestResults)
            {
                var probabilityModifiedByPercentageOfSplit = child.Item1.InstancesPercentage * probabilitiesProductSoFar;
                var linkFollowingResults = this.ProcessInstance(vector, child.Item2, probabilityModifiedByPercentageOfSplit);
                if (!results.ContainsKey(linkFollowingResults.Item1))
                {
                    results.Add(linkFollowingResults.Item1, 0);
                }
                results[linkFollowingResults.Item1] += linkFollowingResults.Item2;
            }
            if (!results.Any())
            {
                return new Tuple<TDecisionValue, double>(default(TDecisionValue), 0);
            }
            var normalizer = results.Values.Sum();
            var winningOption =
                results.Select(
                    res => new { DecisionValue = res.Key, ProbbailityOfSelection = res.Value / normalizer })
                    .OrderByDescending(res => res.ProbbailityOfSelection)
                    .First();
            return new Tuple<TDecisionValue, double>(winningOption.DecisionValue, winningOption.ProbbailityOfSelection);
        }
    }
}