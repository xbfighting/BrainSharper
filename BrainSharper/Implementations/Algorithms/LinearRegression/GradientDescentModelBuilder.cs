namespace BrainSharper.Implementations.Algorithms.LinearRegression
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Runtime.InteropServices;

    using Abstract.Algorithms.Infrastructure;
    using Abstract.Algorithms.LinearRegression;
    using Abstract.Data;

    using BrainSharper.General.Utils;

    using MathNet.Numerics;
    using MathNet.Numerics.LinearAlgebra;

    public class GradientDescentModelBuilder : ILinearRegressionModelBuilder
    {
        private readonly int iterations;
        private readonly Random randomizer;
        private readonly double weightsMin;
        private readonly double weightsMax;
        private readonly double stopLerningError;

        public GradientDescentModelBuilder(double minWeight = 0, double maxWeight = 0, Random random = null, int iterCount = 1000, double stopThreshold = 0.00001)
        {
            randomizer = random ?? new Random();
            weightsMin = minWeight;
            weightsMax = maxWeight;
            iterations = iterCount;
            stopLerningError = stopThreshold;
        }

        public IPredictionModel BuildModel(IDataFrame dataFrame, string dependentFeatureName, IModelBuilderParams additionalParams)
        {
            if (!(additionalParams is ILinearRegressionParams))
            {
                throw new ArgumentException("Invalid parameters passed to Gradient Desccent model builder!");
            }
            var linearRegressionParams = additionalParams as ILinearRegressionParams;
            var matrixX = dataFrame.GetSubsetByColumns(dataFrame.ColumnNames.Except(new[] { dependentFeatureName }).ToList()).GetAsMatrixWithIntercept();
            var vectorY = dataFrame.GetNumericColumnVector(dependentFeatureName);
            return BuildModel(matrixX, vectorY, linearRegressionParams);
        }

        public IPredictionModel BuildModel(IDataFrame dataFrame, int dependentFeatureIndex, IModelBuilderParams additionalParams)
        {
            return BuildModel(dataFrame, dataFrame.ColumnNames[dependentFeatureIndex], additionalParams);
        }

        public ILinearRegressionModel BuildModel(Matrix<double> matrixX, Vector<double> vectorY, ILinearRegressionParams regressionParams)
        {
            var weightsVector = Vector<double>.Build.DenseOfArray(
                Enumerable.Range(0, matrixX.ColumnCount)
                .Select(fIdx => randomizer.NextDoubleInRange(weightsMin, weightsMax))
                .ToArray());
            double previousIterError = double.PositiveInfinity;
            for (int iterNo = 0; iterNo < iterations; iterNo++)
            {
                var vectorYHat = matrixX.Multiply(weightsVector);
                var diffVector = vectorYHat.Subtract(vectorY);
                var currentIterError = diffVector.Select(Math.Abs).Sum();
                if ((currentIterError <= stopLerningError) ||
                    currentIterError.AlmostEqual(previousIterError, 5))
                {
                    break;
                }
                for (int weightIdx = 0; weightIdx < weightsVector.Count; weightIdx++)
                {
                    var diffSums = 0.0;
                    for (int rowIdx = 0; rowIdx < matrixX.RowCount; rowIdx++)
                    {
                        diffSums += diffVector[rowIdx] * matrixX.Row(rowIdx)[weightIdx];
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
            return currentWeightValue - (((regressionParams.LearningRate * 1) / matrixX.RowCount) * diffSums);
        }
    }
}
