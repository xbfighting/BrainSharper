using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using BrainSharper.General.DataQuality;
using BrainSharper.General.MathFunctions;
using BrainSharper.General.Utils;
using BrainSharper.Implementations.Algorithms.Knn;
using BrainSharper.Implementations.Algorithms.Knn.BackwardsElimination;
using BrainSharper.Implementations.Data;
using BrainSharper.Implementations.MathUtils.DistanceMeasures;
using BrainSharper.Implementations.MathUtils.ErrorMeasures;
using BrainSharper.Implementations.MathUtils.Normalizers;
using BrainSharperTests.TestUtils;
using MathNet.Numerics.LinearAlgebra;
using MathNet.Numerics.Random;
using NUnit.Framework;

namespace BrainSharperTests.Implementations.Algorithms.Knn
{
    [TestFixture]
    public class BackwardsEliminationKnnPredictorTests
    {
        [Test]
        public void Test_RegressionWith_BackwardsEliminationKnnModel()
        {

            // Given
            var randomizer = new Random(55);
            var baseDataFrame = TestDataBuilder.BuildRandomAbstractNumericDataFrame(randomizer: randomizer, rowCount: 1000);

            var queryDataFrame = new DataFrame(new DataTable("some data")
            {
                Columns =
                {
                    new DataColumn("F1", typeof(double)),
                    new DataColumn("F2", typeof(double)),
                    new DataColumn("F3", typeof(double)),
                    new DataColumn("F4", typeof(double)),
                    new DataColumn("F5", typeof(double))
                },
                Rows =
                {
                   new object[] { 10, 1, 1, 4, 5 },
                   new object[] { 4, 2, 1, 9, 10},
                   new object[] { 2, 1, 1, 3, 7},
                }
            });
            var expectedValues = Enumerable.Range(0, queryDataFrame.RowCount)
                .Select(
                    rowIdx =>
                        TestDataBuilder.CalcualteLinearlyDependentFeatureValue(queryDataFrame.GetNumericRowVector(rowIdx))).ToList();
            var weightingFunction = new GaussianFunction(0.07);
            var predictor = new SimpleKnnRegressor(
                new EuclideanDistanceMeasure(),
                new MinMaxNormalizer(),
                weightingFunction.GetValue);
            var modelBuilder = new BackwardsEliminationKnnModelBuilder<double>(
                new MinMaxNormalizer(),
                predictor,
                new MeanSquareError()
                );
            var modelParams = new KnnAdditionalParams(3, true);
            var errorMeasure = new MeanSquareError();

            var subject = new BackwardsEliminationKnnRegressor(
                new EuclideanDistanceMeasure(),
                new MinMaxNormalizer(),
                weightingFunction.GetValue);

            // When
            var model = modelBuilder.BuildModel(baseDataFrame, "F6", modelParams);
            var actualOutcomes = subject.Predict(queryDataFrame, model, "F6");
            var mse = errorMeasure.CalculateError(Vector<double>.Build.DenseOfEnumerable(expectedValues), Vector<double>.Build.DenseOfEnumerable(actualOutcomes));
            Assert.IsTrue(mse < 0.35);
        }

        [Test]
        public void Test_ClassificationWith_BackwardsEliminationKnnModel()
        {
            // Given
            var randomizer = new Random(55);
            var data = TestDataBuilder.ReadIrisData();
            var trainingDataPercentage = 0.8;
            int trainingDataCount = (int)(data.RowCount * trainingDataPercentage);
            var shuffledIndices = data.RowIndices.Shuffle();
            var trainingIndices = shuffledIndices.Take(trainingDataCount).ToList();
            var testIndices =
                shuffledIndices.Skip(trainingDataCount).Take(shuffledIndices.Count - trainingDataCount).ToList();

            var trainingData = data.GetSubsetByRows(trainingIndices);
            var testData = data.GetSubsetByRows(testIndices);

            var weightingFunction = new GaussianFunction(0.07);
            var predictor = new SimpleKnnClassifier<string>(
                new EuclideanDistanceMeasure(),
                new MinMaxNormalizer(),
                weightingFunction.GetValue);
            var modelBuilder = new BackwardsEliminationKnnModelBuilder<string>(
                new MinMaxNormalizer(),
                predictor,
                new ClassificationAccuracyError<string>()
                );
            var modelParams = new KnnAdditionalParams(3, true);
            var errorMeasure = new MeanSquareError();

            var subject = new BackwardsEliminationKnnClassifier<string>(
                new EuclideanDistanceMeasure(),
                new MinMaxNormalizer(),
                weightingFunction.GetValue);

            // When
            var model = modelBuilder.BuildModel(trainingData, "iris_class", modelParams);
            var actualResults = subject.Predict(testData, model, "iris_class");
            var confusionMatrix = new ConfusionMatrix<string>(testData.GetColumnVector<string>("iris_class"),
                actualResults);
            
            // Then
            Assert.IsTrue(confusionMatrix.Accuracy >= 0.95);
        }
    }
}
