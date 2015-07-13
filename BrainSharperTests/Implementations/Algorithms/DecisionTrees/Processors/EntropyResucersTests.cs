using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
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
        private readonly InformationGainCalculator<string> _informationGainCalculator = new InformationGainCalculator<string>(new ShannonEntropy<string>());
            
        [Test]
        public void TestInformationGainCalculator()
        {
            // Given
            var initialData = TestDataBuilder.ReadWeatherData();
            var subset1 = initialData.GetSubsetByRows(new[] { 0,1,7,8,10 });
            var subset2 = initialData.GetSubsetByRows(new[] { 2, 6, 11, 12 });
            var subset3 = initialData.GetSubsetByRows(new[] { 3, 4, 5, 9, 13 });

            var splitResults = new List<ISplittingResult>
            {
                new SplittingResult(new DecisionLink(0, 0), subset1),
                new SplittingResult(new DecisionLink(0, 0), subset2),
                new SplittingResult(new DecisionLink(0, 0), subset3)
            };

            var expectedGain = 0.246;

            // When
            var actualGain = _informationGainCalculator.CalculateSplitQuality(initialData, splitResults,
                TestDataBuilder.WeatherDataDependentFeatureName);

            // Then
            Assert.AreEqual(expectedGain, actualGain, 0.0009);
        }
    }
}
