using System;
using System.Data;
using System.Linq;
using BrainSharper.General.MathFunctions;
using BrainSharper.Implementations.Algorithms.Knn;
using BrainSharper.Implementations.Algorithms.Knn.BackwardsElimination;
using BrainSharper.Implementations.Data;
using BrainSharper.Implementations.MathUtils.DistanceMeasures;
using BrainSharper.Implementations.MathUtils.ErrorMeasures;
using BrainSharper.Implementations.MathUtils.Normalizers;
using BrainSharperTests.TestUtils;
using MathNet.Numerics.LinearAlgebra;
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
            var weightingFunction = new GaussianFunction(0.3);
            var predictor = new SimpleKnnRegressor(
                new EuclideanDistanceMeasure(), 
                new MinMaxNormalizer(), 
                weightingFunction.GetValue);
            var modelBuilder = new BackwardsEliminationKnnModelBuilder<double>(
                new MinMaxNormalizer(),
                predictor,
                new MeanSquareError()
                );
            var modelParams = new KnnAdditionalParams(4, true);
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
    }
}
