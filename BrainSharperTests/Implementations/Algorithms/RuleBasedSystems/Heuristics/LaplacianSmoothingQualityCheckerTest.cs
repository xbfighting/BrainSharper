namespace BrainSharperTests.Implementations.Algorithms.RuleBasedSystems.Heuristics
{
    using System.Linq;

    using BrainSharper.Abstract.Algorithms.RuleInduction.DataStructures;
    using BrainSharper.Abstract.Data;
    using BrainSharper.Implementations.Algorithms.RuleInduction.DataStructures;
    using BrainSharper.Implementations.Algorithms.RuleInduction.Heuristics;

    using BrainSharperTests.TestUtils;

    using NUnit.Framework;

    [TestFixture]
    public class LaplacianSmoothingQualityCheckerTest
    {
        private readonly IDataFrame testData = TestDataBuilder.ReadWeatherDataWithCategoricalAttributes();
        private readonly LaplacianSmoothingQualityChecker subject = new LaplacianSmoothingQualityChecker(2, 2);
            
        [Test]
        public void LaplacianSmoothingQualityChecker_TestValues()
        {
            // Given
            var complex = new Complex<string>(
                new ISelector<string>[]
                    {
                        new DisjunctiveSelector<string>("Outlook", "Sunny", "Overcast")
                    });
            var coveredExamplesIndices = this.testData.RowIndices.Where(rowIdx => complex.Covers(this.testData.GetRowVector<string>(rowIdx))).ToList();
            var expectedQuality = 0.636;

            // When
            var actualQuality = subject.CalculateComplexQuality(
                testData,
                TestDataBuilder.WeatherDataDependentFeatureName,
                coveredExamplesIndices);

            // Then
            Assert.AreEqual(expectedQuality, actualQuality.QualityValue, 0.009);
        }
    }
}
