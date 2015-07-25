using System.Collections.Generic;
using BrainSharper.Abstract.Algorithms.DecisionTrees.DataStructures;
using BrainSharper.Implementations.Algorithms.DecisionTrees.DataStructures;
using BrainSharper.Implementations.Algorithms.DecisionTrees.Processors;
using BrainSharper.Implementations.MathUtils.ImpurityMeasures;
using BrainSharperTests.TestUtils;
using NUnit.Framework;

namespace BrainSharperTests.Implementations.Algorithms.DecisionTrees.Processors
{
    [TestFixture]
    public class EntropyResucersTests
    {
        private readonly ShannonEntropy<string> _shannonEntropy = new ShannonEntropy<string>();
        private readonly InformationGainCalculator<string, string> _informationGainCalculator;
        private readonly InformationGainCalculator<string, string> _informationGainRatioCalculator;

        public EntropyResucersTests()
        {
            _informationGainCalculator = new InformationGainCalculator<string, string>(_shannonEntropy, _shannonEntropy);
            _informationGainRatioCalculator = new InformationGainRatioCalculator<string, string>(_shannonEntropy, _shannonEntropy);
        }

        [Test]
        public void TestInformationGainCalculator_MultipleSplitByOvercast()
        {
            // Given
            var initialData = TestDataBuilder.ReadWeatherDataWithCategoricalAttributes();
            var subset1 = initialData.GetSubsetByRows(new[] { 0, 1, 7,8,10 });
            var subset2 = initialData.GetSubsetByRows(new[] { 2, 6, 11, 12 });
            var subset3 = initialData.GetSubsetByRows(new[] { 3, 4, 5, 9, 13 });

            var splitResults = new List<ISplittedData<string>>
            {
                new SplittedData<string>(new DecisionLink<string>(0, 0, "sunny"), subset1),
                new SplittedData<string>(new DecisionLink<string>(0, 0, "overcast"), subset2),
                new SplittedData<string>(new DecisionLink<string>(0, 0, "rainy"), subset3)
            };

            var expectedGain = 0.246;

            // When
            var actualGain = _informationGainCalculator.CalculateSplitQuality(initialData, splitResults,
                TestDataBuilder.WeatherDataDependentFeatureName);

            // Then
            Assert.AreEqual(expectedGain, actualGain, 0.0009);
        }

        [Test]
        public void TestInformationGainRatioCalculator()
        {
            // Given
            var initialData = TestDataBuilder.ReadWeatherDataWithCategoricalAttributes();
            var subset1 = initialData.GetSubsetByRows(new[] { 0, 1, 7, 8, 10 });
            var subset2 = initialData.GetSubsetByRows(new[] { 2, 6, 11, 12 });
            var subset3 = initialData.GetSubsetByRows(new[] { 3, 4, 5, 9, 13 });

            var splitResults = new List<ISplittedData<string>>
            {
                new SplittedData<string>(new DecisionLink<string>(0, 0, "sunny"), subset1),
                new SplittedData<string>(new DecisionLink<string>(0, 0, "overcast"), subset2),
                new SplittedData<string>(new DecisionLink<string>(0, 0, "rainy"), subset3)
            };

            var expectedGain = 0.156;

            // When
            var actualGain = _informationGainRatioCalculator.CalculateSplitQuality(initialData, splitResults,
                TestDataBuilder.WeatherDataDependentFeatureName);

            // Then
            Assert.AreEqual(expectedGain, actualGain, 0.0009);
        }
    }
}
