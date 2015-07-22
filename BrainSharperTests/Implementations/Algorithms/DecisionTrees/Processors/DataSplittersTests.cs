using System.Linq;
using BrainSharper.Abstract.Algorithms.DecisionTrees.Processors;
using BrainSharper.Implementations.Algorithms.DecisionTrees.DataStructures.BinaryDecisionTrees;
using BrainSharper.Implementations.Algorithms.DecisionTrees.Processors;
using BrainSharperTests.TestUtils;
using NUnit.Framework;

namespace BrainSharperTests.Implementations.Algorithms.DecisionTrees.Processors
{
    [TestFixture]
    public class DataSplittersTests
    {
        private static readonly IBinaryDataSplitter<string> _binaryDiscreteDataSplitter = new BinaryDiscreteDataSplitter<string>();
        private static readonly IBinaryDataSplitter<double> _binaryNumericDataSplitter = new BinaryNumericDataSplitter();

        [Test]
        public void PerformBinaryDiscreteDataSplit()
        {
            // Given
            var testData = TestDataBuilder.ReadWeatherDataWithCategoricalAttributes();
            var expectedPositiveData = testData.GetSubsetByRows(new[] { 3, 4, 5, 9, 13 });
            var expectedNegativeData = testData.GetSubsetByRows(new[] { 0, 1, 2, 6, 7, 8, 10, 11, 12 });


            var splitCriteria = new BinarySplittingParams<string>("Outlook", "Rainy");

            // When
            var splitResults = _binaryDiscreteDataSplitter.SplitData(testData, splitCriteria);
            var actualPositiveData = splitResults.First().SplittedDataFrame;
            var actualNegativeData = splitResults.Last().SplittedDataFrame;

            // Then
            Assert.IsTrue(expectedPositiveData.Equals(actualPositiveData));
            Assert.IsTrue(actualNegativeData.Equals(expectedNegativeData));
        }

        [Test]
        public void PerformBinaryNumericDataSplit()
        {
            // Given
            var testData = TestDataBuilder.BuildSmallDataFrameNumbersOnly();
            var expectedPositiveData = testData.GetSubsetByRows(new[] { 1, 2 });
            var expectedNegativeData = testData.GetSubsetByRows(new[] {0});
            var splitCriterion = new BinarySplittingParams<double>("Col1", 5);

            // When
            var splitResults = _binaryNumericDataSplitter.SplitData(testData, splitCriterion);
            var actualPositiveData = splitResults.First().SplittedDataFrame;
            var actualNegativeData = splitResults.Last().SplittedDataFrame;

            // Then
            Assert.IsTrue(expectedPositiveData.Equals(actualPositiveData));
            Assert.IsTrue(actualNegativeData.Equals(expectedNegativeData));
        }
    }
}
