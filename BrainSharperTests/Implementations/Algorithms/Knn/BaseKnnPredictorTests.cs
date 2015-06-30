using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using BrainSharper.General.MathFunctions;
using BrainSharper.Implementations.Algorithms.Knn;
using BrainSharper.Implementations.Data;
using BrainSharper.Implementations.MathUtils;
using BrainSharper.Implementations.MathUtils.DistanceMeasures;
using BrainSharper.Implementations.MathUtils.ErrorMeasures;
using BrainSharper.Implementations.MathUtils.Normalizers;
using BrainSharperTests.TestUtils;
using MathNet.Numerics.LinearAlgebra;
using NUnit.Framework;

namespace BrainSharperTests.Implementations.Algorithms.Knn
{
    [TestFixture]
    public class BaseKnnPredictorTests
    {
        [Test]
        public void TestBuildKnnModel()
        {
            // Given
            var randomizer = new Random(55);
            var baseDataFrame = TestDataBuilder.BuildRandomAbstractNumericDataFrame(randomizer: randomizer);

            // TODO: change data type in query frame
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
            

            var modelBuilder = new BaseKnnModelBuilder();
            var modelParams = new KnnAdditionalParams(4, true);
            var weightingFunction = new GaussianFunction(0.3);
            var predictor = new BaseKnnPredictor(new EuclideanDistanceMeasure(), new MinMaxNormalizer(), weightingFunction.GetValue);
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
