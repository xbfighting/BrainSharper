namespace BrainSharper.Implementations.Algorithms.DecisionTrees.Processors
{
    using System;
    using System.Linq;

    using Abstract.Algorithms.DecisionTrees.DataStructures;
    using Abstract.Algorithms.DecisionTrees.Processors;
    using Abstract.Data;

    using BrainSharper.Abstract.Algorithms.LinearRegression;
    using BrainSharper.Implementations.Algorithms.LinearRegression;

    using DataStructures;

    using MathNet.Numerics.LinearAlgebra;
    using MathNet.Numerics.LinearRegression;
    using MathNet.Numerics.Statistics;

    public class RegressionAndModelDecisionTreeLeafBuilder : ILeafBuilder
    {
        private readonly ILinearRegressionModelBuilder regressionModelBuilder;
        private readonly ILinearRegressionParams regressionParams;

        public RegressionAndModelDecisionTreeLeafBuilder(ILinearRegressionModelBuilder modelBuilder, double learningRate = 0.05)
        {
            regressionModelBuilder = modelBuilder;
            regressionParams = new LinearRegressionParams(learningRate);
        }

        public IDecisionTreeLeaf BuildLeaf(IDataFrame finalData, string dependentFeatureName)
        {
            var vectorY = finalData.GetNumericColumnVector(dependentFeatureName);
            var featureNames = finalData.ColumnNames.Except(new[] { dependentFeatureName }).ToList();
            var subset = finalData.GetSubsetByColumns(featureNames);
            var matrixX = finalData.GetSubsetByColumns(featureNames).GetAsMatrixWithIntercept();
            Vector<double> fittedWeights = null;
            
            try
            {
                fittedWeights = MultipleRegression.DirectMethod(matrixX, vectorY);
            }   
            catch (Exception exc)
            {
                fittedWeights = regressionModelBuilder.BuildModel(matrixX, vectorY, regressionParams).Weights;
            }
            
            return new RegressionAndModelLeaf(dependentFeatureName, fittedWeights, vectorY.Mean());
        }
    }
}
