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
        private readonly IDataSplitter<string> _multiValueCategoricalDataSplitter; 

        private readonly IBinaryBestSplitSelector _binaryBestSplitSelector;
        private readonly IBestSplitSelector _multiValueBestSplitSelector; 


        private readonly ISplitQualityChecker _categoricalBinarySplitQualityChecker;
        private readonly ISplitQualityChecker _categoricalMultiValueSplitQualityChecker;

        public BestSplitSelectorsTests()
        {
            ICategoricalImpurityMeasure<string> shannonEntropy = new ShannonEntropy<string>();
            IBinaryDataSplitter<string> binaryDataSplitter = new BinaryDiscreteDataSplitter<string>();
            _binaryNumericDataSplitter = new BinaryNumericDataSplitter();
            _binaryBestSplitSelector = new BinarySplitSelector<string>(binaryDataSplitter, _binaryNumericDataSplitter);
            _categoricalBinarySplitQualityChecker = new InformationGainCalculator<string>(shannonEntropy, shannonEntropy);
            _categoricalMultiValueSplitQualityChecker = new InformationGainCalculator<string>(shannonEntropy, shannonEntropy);
            _multiValueCategoricalDataSplitter = new MultiValueDiscreteDataSplitter<string>();
            _multiValueBestSplitSelector = new MultiValueSplitSelectorForCategoricalOutcome<string>(_multiValueCategoricalDataSplitter, _binaryNumericDataSplitter);
        }

        [Test]
        public void SelectBestSplit_BinarySplit_CategoricalAttributes_CategoricalOutput()
        {
            // Given
            var testData = TestDataBuilder.ReadWeatherDataWithCategoricalAttributes();
            
            // When
            var bestSplit = _binaryBestSplitSelector.SelectBestSplit(testData, "Play", _categoricalBinarySplitQualityChecker);

            // Then
            Assert.IsNotNull(bestSplit);
        }

        [Test]
        public void SelectBestSplit_BinarySplit_NumericAttributes_CategoricalOutput()
        {
            // Given
            var testData = TestDataBuilder.ReadWeatherDataWithMixedAttributes();
            var subject = new BinarySplitSelector<object>(new BinaryDiscreteDataSplitter<object>(), _binaryNumericDataSplitter);

            // When
            var bestSplit = subject.SelectBestSplit(testData, "Play", _categoricalBinarySplitQualityChecker);
        }

        [Test]
        public void SelectBestSplit_MultiValue_CategoricalAtributes_CategoricalOutput()
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
