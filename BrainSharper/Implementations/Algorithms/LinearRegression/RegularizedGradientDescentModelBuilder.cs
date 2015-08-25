namespace BrainSharper.Implementations.Algorithms.LinearRegression
{
    using System;
    using Abstract.Algorithms.LinearRegression;
    using MathNet.Numerics.LinearAlgebra;

    public class RegularizedGradientDescentModelBuilder : GradientDescentModelBuilder
    {
        private readonly double regularizationConst;

        public RegularizedGradientDescentModelBuilder(
            double minWeight = 0, 
            double maxWeight = 0, 
            Random random = null, 
            int iterCount = 1000, 
            double regularizationVal = 0.5,
            double stopThreshold = 0.00001)
            : base(minWeight, maxWeight, random, iterCount, stopThreshold)
        {
            regularizationConst = regularizationVal;
        }

        protected override double CalculateWeightUpdate(
            Matrix<double> matrixX, 
            ILinearRegressionParams regressionParams, 
            double diffSums,
            double currentWeightValue)
        {
            var update = base.CalculateWeightUpdate(matrixX, regressionParams, diffSums, currentWeightValue);
            var regularizationTerm = (regularizationConst / matrixX.RowCount) * currentWeightValue;
            return update - regularizationTerm;
        }
    }
}
