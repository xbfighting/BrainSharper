using System;
using System.Collections.Generic;
using System.Linq;
using BrainSharper.Abstract.Algorithms.Infrastructure;
using BrainSharper.Abstract.Algorithms.LinearRegression;
using BrainSharper.Abstract.Data;

namespace BrainSharper.Implementations.Algorithms.LinearRegression
{
    public class LinearRegressionPredictor : ILinearRegressionPredictor
    {
        public IList<double> Predict(IDataFrame queryDataFrame, IPredictionModel model, string dependentFeatureName)
        {
            if (!(model is ILinearRegressionModel))
            {
                throw new ArgumentException("Invalid model passed to Linear Regression predictor!");
            }
            var linearRegressionModel = model as ILinearRegressionModel;
            var xMatrix = queryDataFrame.GetSubsetByColumns(
                queryDataFrame.ColumnNames.Except(new[] {dependentFeatureName}).ToList())
                .GetAsMatrixWithIntercept();
            var results = new List<double>();
            for (var rowIdx = 0; rowIdx < xMatrix.RowCount; rowIdx++)
            {
                var queryRow = xMatrix.Row(rowIdx);
                var result = linearRegressionModel.Weights.DotProduct(queryRow);
                results.Add(result);
            }
            return results;
        }

        public IList<double> Predict(IDataFrame queryDataFrame, IPredictionModel model, int dependentFeatureIndex)
        {
            return Predict(queryDataFrame, model, queryDataFrame.ColumnNames[dependentFeatureIndex]);
        }
    }
}