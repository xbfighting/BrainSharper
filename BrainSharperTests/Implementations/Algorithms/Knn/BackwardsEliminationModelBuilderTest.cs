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
    public class SimpleKnnPredictorTests
    {
        [Test]
        public void TestBuildKnnModel()
        {
            // Given
            var randomizer = new Random(55);
            var baseDataFrame = TestDataBuilder.BuildRandomAbstractNumericDataFrame(randomizer: randomizer, rowCount: 100);

            var weightingFunction = new GaussianFunction(0.3);
            var modelParams = new KnnAdditionalParams(4, true);
            var predictor = new SimpleKnnPredictor(new EuclideanDistanceMeasure(), new MinMaxNormalizer(), weightingFunction.GetValue);
            var subject = new BackwardsEliminationKnnModelBuilder(
                new MinMaxNormalizer(),
                predictor,
                new MeanSquareError());

            // When
            var model = subject.BuildModel(baseDataFrame, "F6", modelParams);
        }

    }
}
