namespace BrainSharperTests.Implementations.Algorithms.LinearRegression
{
    using System;
    using System.Linq;
    using System.Collections.Generic;
    using System.Threading;

    using BrainSharper.Abstract.Algorithms.LinearRegression;
    using BrainSharper.General.DataQuality;
    using BrainSharper.General.DataUtils;
    using BrainSharper.Implementations.Algorithms.LinearRegression;

    using BrainSharperTests.TestUtils;

    using MathNet.Numerics.LinearAlgebra;

    using NUnit.Framework;

    [TestFixture]
    public class LinearRegressionTests
    {
        [Test]
        public void GradientDescent_ArtificialFunction()
        {
            // Given
            var splitter = new CrossValidator<double>();
            Func<IList<double>, double> scoreFunc = list => 0.3 + (0.5 * list[0]) + (-0.3 * list[1]) + (0.7 * list[2]);
            var allData =
                TestDataBuilder.BuildRandomAbstractNumericDataFrame(
                    scoreFunc,
                    featuresCount: 3,
                    min: 0,
                    max: 1,
                    rowCount: 1000);
            var subject = new GradientDescentModelBuilder(0, 1);
            var regParams = new LinearRegressionParams(0.05);

            // When
            var accuracies = splitter.CrossValidate(
               modelBuilder: subject,
               modelBuilderParams: regParams,
               predictor: new LinearRegressionPredictor(), 
               qualityMeasure: new GoodnessOfFitQualityMeasure(),
               dataFrame: allData,
               dependentFeatureName: "result",
               percetnagOfTrainData: 0.8,
               folds: 20);

            // Then
            Assert.IsTrue(accuracies.Select(acc => acc.Accuracy).Average() >= 0.9);
        }

        [Test]
        public void RegularizedGradientDescent_ArtificialFunction()
        {
            // Given
            var splitter = new CrossValidator<double>();
            Func<IList<double>, double> scoreFunc = list => 0.3 + (0.5 * list[0]) + (-0.3 * list[1]) + (0.7 * list[2]);
            var allData =
                TestDataBuilder.BuildRandomAbstractNumericDataFrame(
                    scoreFunc,
                    featuresCount: 3,
                    min: 0,
                    max: 1,
                    rowCount: 1000);
            var subject = new RegularizedGradientDescentModelBuilder(0, 1);
            var regParams = new LinearRegressionParams(0.05);

            // When
            var accuracies = splitter.CrossValidate(
               modelBuilder: subject,
               modelBuilderParams: regParams,
               predictor: new LinearRegressionPredictor(),
               qualityMeasure: new GoodnessOfFitQualityMeasure(),
               dataFrame: allData,
               dependentFeatureName: "result",
               percetnagOfTrainData: 0.8,
               folds: 20);

            // Then
            Assert.IsTrue(accuracies.Select(acc => acc.Accuracy).Average() >= 0.9);
        }

        [Test]
        public void RegularizedLinearRegression_ArtificaialFunction()
        {
            // Given
            var splitter = new CrossValidator<double>();
            Func<IList<double>, double> scoreFunc = list => 0.3 + (0.5 * list[0]) + (-0.3 * list[1]) + (0.7 * list[2]);
            var allData =
                TestDataBuilder.BuildRandomAbstractNumericDataFrame(
                    scoreFunc,
                    featuresCount: 3,
                    min: 0,
                    max: 1,
                    rowCount: 1000);
            var subject = new RegularizedLinearRegressionModelBuilder(0.5);
            var regParams = new LinearRegressionParams(0.05);

            // When
            var accuracies = splitter.CrossValidate(
               modelBuilder: subject,
               modelBuilderParams: regParams,
               predictor: new LinearRegressionPredictor(),
               qualityMeasure: new GoodnessOfFitQualityMeasure(),
               dataFrame: allData,
               dependentFeatureName: "result",
               percetnagOfTrainData: 0.8,
               folds: 20);

            // Then
            Assert.IsTrue(accuracies.Select(acc => acc.Accuracy).Average() >= 0.9);
        }
    }
}
