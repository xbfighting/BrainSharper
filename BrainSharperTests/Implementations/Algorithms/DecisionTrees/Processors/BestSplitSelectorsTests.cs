using System.Collections.Generic;
using BrainSharper.Abstract.Algorithms.DecisionTrees.Processors;
using BrainSharper.Abstract.MathUtils.ImpurityMeasures;
using BrainSharper.Implementations.Algorithms.DecisionTrees.Processors;
using BrainSharper.Implementations.MathUtils.ImpurityMeasures;
using BrainSharperTests.TestUtils;
using NUnit.Framework;

namespace BrainSharperTests.Implementations.Algorithms.DecisionTrees.Processors
{
    [TestFixture]
    public class BestSplitSelectorsTests
    {
        private readonly IBinaryNumericDataSplitter _binaryNumericDataSplitter;
        private readonly IDataSplitter _multiValueCategoricalDataSplitter;

        private readonly IBinaryBestSplitSelector _binaryBestSplitSelector;
        private readonly IBestSplitSelector _multiValueBestSplitSelector;

        private readonly IBinaryNumericAttributeBestSplitPointSelector _binaryNumericBestSplitPointSelector;

        private readonly ISplitQualityChecker _categoricalBinarySplitQualityChecker;
        private readonly ISplitQualityChecker _categoricalMultiValueSplitQualityChecker;

        

        public BestSplitSelectorsTests()
        {
            ICategoricalImpurityMeasure<string> shannonEntropy = new ShannonEntropy<string>();
            IBinaryDataSplitter binaryDataSplitter = new BinaryDiscreteDataSplitter();
            _binaryNumericDataSplitter = new BinaryNumericDataSplitter();
            _binaryNumericBestSplitPointSelector = new ClassBreakpointsNumericSplitFinder();
            _binaryBestSplitSelector = new BinarySplitSelectorForCategoricalOutcome(binaryDataSplitter, _binaryNumericDataSplitter, _binaryNumericBestSplitPointSelector);
            _categoricalBinarySplitQualityChecker = new InformationGainCalculator<string>(shannonEntropy, shannonEntropy);
            _categoricalMultiValueSplitQualityChecker = new InformationGainCalculator<string>(shannonEntropy, shannonEntropy);
            _multiValueCategoricalDataSplitter = new MultiValueDiscreteDataSplitter();
            _multiValueBestSplitSelector = new MultiValueSplitSelectorForCategoricalOutcome(_multiValueCategoricalDataSplitter, _binaryNumericDataSplitter, _binaryNumericBestSplitPointSelector);
        }

        [Test]
        public void SelectBestSplitBinarySplitCategoricalAttributesCategoricalOutput()
        {
            // Given
            var testData = TestDataBuilder.ReadWeatherDataWithCategoricalAttributes();

            // When
            var bestSplit = _binaryBestSplitSelector.SelectBestSplit(testData, "Play", _categoricalBinarySplitQualityChecker);

            // Then
            Assert.IsNotNull(bestSplit);
        }

        [Test]
        public void SelectBestSplitBinarySplitNumericAttributesCategoricalOutput()
        {
            // Given
            var testData = TestDataBuilder.ReadWeatherDataWithMixedAttributes().GetSubsetByColumns(new[] { "Temperature", "Humidity", "Windy", "Play" });
            var subject = new BinarySplitSelectorForCategoricalOutcome(new BinaryDiscreteDataSplitter(), _binaryNumericDataSplitter, _binaryNumericBestSplitPointSelector);

            // When
            var bestSplit = subject.SelectBestSplit(testData, "Play", _categoricalBinarySplitQualityChecker);
        }

        [Test]
        public void SelectBestSplitMultiValueCategoricalAtributesCategoricalOutput()
        {
            // Given
            var testData = TestDataBuilder.ReadWeatherDataWithCategoricalAttributes();
            var expectedBestSplitAttribute = "Outlook";
            var expectedRowCountsPerAttribute = new Dictionary<object, int>
            {
                ["Sunny"] = 5,
                ["Overcast"] = 4,
                ["Rainy"] = 5
            };

            // When
            var bestSplit = _multiValueBestSplitSelector.SelectBestSplit(testData, "Play", _categoricalMultiValueSplitQualityChecker);

            // Then
            Assert.AreEqual(expectedBestSplitAttribute, bestSplit.SplittingFeatureName);
            Assert.AreEqual(expectedRowCountsPerAttribute.Count, bestSplit.SplittedDataSets.Count);
            foreach (var splittedData in bestSplit.SplittedDataSets)
            {
                var expectedRowCounts = expectedRowCountsPerAttribute[splittedData.SplitLink.TestResult];
                Assert.AreEqual(expectedRowCounts, splittedData.SplittedDataFrame.RowCount);
            }
        }
    }
}
