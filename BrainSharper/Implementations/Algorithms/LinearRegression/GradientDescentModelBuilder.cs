using System;
using System.Linq;
using BrainSharper.Abstract.Algorithms.Infrastructure;
using BrainSharper.Abstract.Algorithms.LinearRegression;
using BrainSharper.Abstract.Data;
using BrainSharper.General.Utils;
using MathNet.Numerics;
using MathNet.Numerics.LinearAlgebra;

namespace BrainSharper.Implementations.Algorithms.LinearRegression
{
    public class GradientDescentModelBuilder : ILinearRegressionModelBuilder
    {
        private readonly int iterations;
        private readonly Random randomizer;
        private readonly double stopLerningError;
        private readonly double weightsMax;
        private readonly double weightsMin;

        public GradientDescentModelBuilder(double minWeight = 0, double maxWeight = 0, Random random = null,
            int iterCount = 1000, double stopThreshold = 0.00001)
        {
            randomizer = random ?? new Random();
            weightsMin = minWeight;
            weightsMax = maxWeight;
            iterations = iterCount;
            stopLerningError = stopThreshold;
        }

        public IPredictionModel BuildModel(IDataFrame dataFrame, string dependentFeatureName,
            IModelBuilderParams additionalParams)
        {
            if (!(additionalParams is ILinearRegressionParams))
            {
                throw new ArgumentException("Invalid parameters passed to Gradient Desccent model builder!");
            }
            var linearRegressionParams = additionalParams as ILinearRegressionParams;
            var matrixX =
                dataFrame.GetSubsetByColumns(dataFrame.ColumnNames.Except(new[] {dependentFeatureName}).ToList())
                    .GetAsMatrixWithIntercept();
            var vectorY = dataFrame.GetNumericColumnVector(dependentFeatureName);
            return BuildModel(matrixX, vectorY, linearRegressionParams);
        }

        public IPredictionModel BuildModel(IDataFrame dataFrame, int dependentFeatureIndex,
            IModelBuilderParams additionalParams)
        {
            return BuildModel(dataFrame, dataFrame.ColumnNames[dependentFeatureIndex], additionalParams);
        }

        public ILinearRegressionModel BuildModel(Matrix<double> matrixX, Vector<double> vectorY,
            ILinearRegressionParams regressionParams)
        {
            var weightsVector = Vector<double>.Build.DenseOfArray(
                Enumerable.Range(0, matrixX.ColumnCount)
                    .Select(fIdx => randomizer.NextDoubleInRange(weightsMin, weightsMax))
                    .ToArray());
            var previousIterError = double.PositiveInfinity;
            for (var iterNo = 0; iterNo < iterations; iterNo++)
            {
                var vectorYHat = matrixX.Multiply(weightsVector);
                var diffVector = vectorYHat.Subtract(vectorY);
                var currentIterError = diffVector.Select(Math.Abs).Sum();
                if ((currentIterError <= stopLerningError) ||
                    currentIterError.AlmostEqual(previousIterError, 5))
                {
                    break;
                }
                for (var weightIdx = 0; weightIdx < weightsVector.Count; weightIdx++)
                {
                    var diffSums = 0.0;
                    for (var rowIdx = 0; rowIdx < matrixX.RowCount; rowIdx++)
                    {
                        diffSums += diffVector[rowIdx]*matrixX.Row(rowIdx)[weightIdx];
                    }
                    var newWeightValue = CalculateWeightUpdate(
                        matrixX,
                        regressionParams,
                        diffSums,
                        weightsVector[weightIdx]);
                    weightsVector[weightIdx] = newWeightValue;
                }
                previousIterError = currentIterError;
            }

            return new LinearRegressionModel(weightsVector);
        }

        protected virtual double CalculateWeightUpdate(
            Matrix<double> matrixX,
            ILinearRegressionParams regressionParams,
            double diffSums,
            double currentWeightValue)
        {
            return currentWeightValue - (((regressionParams.LearningRate*1)/matrixX.RowCount)*diffSums);
        }
    }
}