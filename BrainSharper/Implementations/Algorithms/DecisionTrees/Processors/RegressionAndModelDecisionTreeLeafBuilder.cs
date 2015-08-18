namespace BrainSharper.Implementations.Algorithms.DecisionTrees.Processors
{
    using System;
    using System.Linq;

    using Abstract.Algorithms.DecisionTrees.DataStructures;
    using Abstract.Algorithms.DecisionTrees.Processors;
    using Abstract.Data;

    using DataStructures;

    using MathNet.Numerics.LinearAlgebra;
    using MathNet.Numerics.LinearRegression;
    using MathNet.Numerics.Statistics;

    public class RegressionAndModelDecisionTreeLeafBuilder : ILeafBuilder
    {
        public IDecisionTreeLeaf BuildLeaf(IDataFrame finalData, string dependentFeatureName)
        {
            //TODO: AAA !!! 1) handle situation, when matrix cannot be inversed. 2) think about embedding split quality checker into BestSplitSelectors (more customized solutions posssible)!
            var dependentValues = finalData.GetNumericColumnVector(dependentFeatureName);
            Vector<double> fittedWeights = null;
            try
            {
                var featureNames = finalData.ColumnNames.Except(new[] { dependentFeatureName }).ToList();
                var subset = finalData.GetSubsetByColumns(featureNames);
                if (subset.RowCount != finalData.RowCount)
                {
                    Console.WriteLine("here!");
                }
                var featureMatrix = finalData.GetSubsetByColumns(featureNames).GetAsMatrixWithIntercept();
                fittedWeights = MultipleRegression.DirectMethod(featureMatrix, dependentValues);
            }   
            catch (Exception exc)
            {
                Console.WriteLine(exc);
            }
            return new RegressionAndModelLeaf(dependentFeatureName, fittedWeights, dependentValues.Mean());
        }
    }
}
