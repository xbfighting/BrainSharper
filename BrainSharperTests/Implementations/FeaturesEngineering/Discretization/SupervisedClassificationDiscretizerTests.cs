
namespace BrainSharperTests.Implementations.FeaturesEngineering.Discretization
{
    using System.Collections.Generic;

    using BrainSharper.Abstract.FeaturesEngineering;
    using BrainSharper.Implementations.Algorithms.DecisionTrees.Processors;
    using BrainSharper.Implementations.FeaturesEngineering;
    using BrainSharper.Implementations.FeaturesEngineering.Discretization;
    using BrainSharper.Implementations.MathUtils.ImpurityMeasures;

    using BrainSharperTests.TestUtils;

    using NUnit.Framework;

    [TestFixture]
    public class SupervisedClassificationDiscretizerTests
    {
        [Test]
        public void DiscretizeTest_WeatherData()
        {
            // Given
            var weatherData = TestDataBuilder.ReadWeatherDataWithMixedAttributes();
            var subject = new SupervisedClassificationDiscretizerDecisionTreeHeuristic(
                new BinaryNumericDataSplitter(),
                new DynamicProgrammingNumericSplitFinder(),
                new InformationGainRatioCalculator<string>(new GiniIndex<string>(), new GiniIndex<string>()),
                new CategoricalDecisionTreeLeafBuilder());

            var expectedValues = new List<IRange>
                                     {
                                         new Range("Temperature", 17, 17.5),
                                         new Range("Temperature", 17.5, 18.5),
                                         new Range("Temperature", 18.5, 21.5),
                                         new Range("Temperature", 21.5, 22.5),
                                         new Range("Temperature", 22.5, 25.5),
                                         new Range("Temperature", 25.5, 28.5),
                                         new Range("Temperature", 28.5, 30.0)
                                     };

            // When
            var discretizedData = subject.Discretize(
                weatherData,
                TestDataBuilder.WeatherDataDependentFeatureName,
                "Temperature",
                "Temp_disc");

            // Then
            CollectionAssert.AreEquivalent(expectedValues, discretizedData.NewValuesMapping.Keys);
        }
    }
}
