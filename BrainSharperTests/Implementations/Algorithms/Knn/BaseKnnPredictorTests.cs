using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using BrainSharper.Implementations.Algorithms.Knn;
using BrainSharper.Implementations.Data;
using BrainSharper.Implementations.MathUtils;
using BrainSharper.Implementations.MathUtils.DistanceMeasures;
using BrainSharper.Implementations.MathUtils.ErrorMeasures;
using BrainSharperTests.TestUtils;
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
            var baseDataFrame = TestDataBuilder.BuildRandomAbstractNumericDataFrame(min: 1, max: 20);

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
                   new object[] { 1, 2, 3, 4, 5 },
                   new object[] { 6, 7, 8, 9, 10},
                   new object[] { 11, 12, 13, 14, 15},
                }
            });
            var expectedValues = Enumerable.Range(0, queryDataFrame.RowCount)
                .Select(
                    rowIdx =>
                        TestDataBuilder.CalcualteLinearlyDependentFeatureValue(queryDataFrame.GetNumericRowVector(rowIdx))).ToList();
            

            var modelBuilder = new BaseKnnModelBuilder();
            var modelParams = new KnnAdditionalParams(5, true);
            var predictor = new BaseKnnPredictor(new EuclideanDistanceMeasure());
            var errorMeasure = new MeanSquareError();
            // When
            var model = modelBuilder.BuildModel(baseDataFrame, "F6", modelParams);
            var results = predictor.Predict(queryDataFrame, model, "F6");

            // Then
            Assert.IsNotNull(results);
        }
    }
}
