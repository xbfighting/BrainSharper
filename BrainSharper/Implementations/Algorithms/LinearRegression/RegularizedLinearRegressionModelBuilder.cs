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

    public class RegularizedLinearRegressionModelBuilder : ILinearRegressionModelBuilder
    {
        private readonly double regularizationConst;

        public RegularizedLinearRegressionModelBuilder(double regularizationVal = 0.5)
        {
            regularizationConst = regularizationVal;
        }

        public IPredictionModel BuildModel(IDataFrame dataFrame, string dependentFeatureName, IModelBuilderParams additionalParams)
        {
            if (!(additionalParams is ILinearRegressionParams))
            {
                throw new ArgumentException("Invalid parameters passed to Regularized Linear Regression model builder!");
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

        public ILinearRegressionModel BuildModel(Matrix<double> matrixX, Vector<double> vectorY, ILinearRegressionParams linearRegressionParams)
        {
            var normalizationMatrix = Matrix<double>.Build.DenseIdentity(matrixX.ColumnCount, matrixX.ColumnCount) * regularizationConst;
            var xT = matrixX.Transpose();
            var xTx = xT.Multiply(matrixX) + normalizationMatrix;
            var inversedXTX = xTx.Inverse();
            var xTy = xT.Multiply(vectorY);
            return new LinearRegressionModel(inversedXTX.Multiply(xTy));
        }
    }
}
