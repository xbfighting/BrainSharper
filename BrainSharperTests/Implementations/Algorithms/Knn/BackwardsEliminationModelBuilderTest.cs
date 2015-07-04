using System;
using System.Linq;
using BrainSharper.Abstract.Algorithms.Knn;
using BrainSharper.General.MathFunctions;
using BrainSharper.Implementations.Algorithms.Knn;
using BrainSharper.Implementations.MathUtils.DistanceMeasures;
using BrainSharper.Implementations.MathUtils.ErrorMeasures;
using BrainSharper.Implementations.MathUtils.Normalizers;
using BrainSharperTests.TestUtils;
using NUnit.Framework;

namespace BrainSharperTests.Implementations.Algorithms.Knn
{
    [TestFixture]
    public class BackwardsEliminationModelBuilderTest
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
            var expectedRemovedFeaturesNames = new[] {"F4", "F5"};

            // When
            var model = subject.BuildModel(baseDataFrame, "F6", modelParams) as IBackwardsEliminationKnnModel;

            // Then
            Assert.AreEqual(2, model.RemovedFeaturesData.Count);
            CollectionAssert.AreEquivalent(expectedRemovedFeaturesNames, model.RemovedFeaturesData.Select(f => f.FeatureName));
        }

    }
}
