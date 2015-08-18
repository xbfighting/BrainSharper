using System;
using System.Data;
using System.Linq;
using BrainSharper.General.MathFunctions;
using BrainSharper.Implementations.Algorithms.Knn;
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
    public class SimpleKnnPredictorTests
    {
        [Test]
        public void Test_RegressionWith_SimpleKnnModel()
        {
            // Given
            var randomizer = new Random(55);
            var baseDataFrame = TestDataBuilder.BuildRandomAbstractNumericDataFrameWithRedundantAttrs(randomizer: randomizer);

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


            var modelBuilder = new SimpleKnnModelBuilder<double>();
            var modelParams = new KnnAdditionalParams(4, true);
            var weightingFunction = new GaussianFunction(0.3);
            var predictor = new SimpleKnnRegressor(new EuclideanDistanceMeasure(), new MinMaxNormalizer(), weightingFunction.GetValue, normalizeNumericValues: true);
            var errorMeasure = new MeanSquareError();

            // When
            var model = modelBuilder.BuildModel(baseDataFrame, "F6", modelParams);
            var results = predictor.Predict(queryDataFrame, model, "F6");

            // Then
            var mse = errorMeasure.CalculateError(Vector<double>.Build.DenseOfEnumerable(expectedValues), Vector<double>.Build.DenseOfEnumerable(results));
            Assert.IsTrue(mse < 0.55);
        }
    }
}
