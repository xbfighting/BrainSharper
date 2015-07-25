using System.Collections.Generic;
using System.Linq;
using BrainSharper.Abstract.Algorithms.DecisionTrees.Processors;
using BrainSharper.Implementations.Algorithms.DecisionTrees.DataStructures;
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
        private static readonly IDataSplitter<string> _multiValueDiscreteDataSplitter = new MultiValueDiscreteDataSplitter<string>();

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

        [Test]
        public void PerformMultiValueDiscreteDataSplit()
        {
            // Given
            var testData = TestDataBuilder.ReadWeatherDataWithCategoricalAttributes();
            var splitParams = new SplittingParams<string>("Outlook");
            var expectedRowCounts = new Dictionary<string, int>
            {
                ["Sunny"] = 5,
                ["Overcast"] = 4,
                ["Rainy"] = 5
            };

            // When
            var splittedData = _multiValueDiscreteDataSplitter.SplitData(testData, splitParams);

            // Then
            Assert.AreEqual(expectedRowCounts.Count, splittedData.Count);
            foreach (var splittedResult in splittedData)
            {
                var expectedCount = expectedRowCounts[splittedResult.SplitLink.TestResult];
                Assert.AreEqual(expectedCount, splittedResult.SplittedDataFrame.RowCount);
                Assert.AreEqual(expectedCount, splittedResult.SplitLink.InstancesCount);
            }
        }
    }
}
