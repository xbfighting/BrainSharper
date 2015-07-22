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
        private readonly ICategoricalImpurityMeasure<string> _shannonEntropy;
        private readonly IBinaryDataSplitter<string> _binaryDataSplitter;
        private readonly IBinaryNumericDataSplitter _binaryNumericDataSplitter;
        private readonly IBinaryBestSplitSelector _binaryBestSplitSelector;
        private readonly ISplitQualityChecker _categoricalSplitQualityChecker;

        public BestSplitSelectorsTests()
        {
            _shannonEntropy = new ShannonEntropy<string>();
            _binaryDataSplitter = new BinaryDiscreteDataSplitter<string>();
            _binaryNumericDataSplitter = new BinaryNumericDataSplitter();
            _binaryBestSplitSelector = new BinarySplitSelector<string>(_binaryDataSplitter, _binaryNumericDataSplitter);
            _categoricalSplitQualityChecker = new InformationGainCalculator<string>(_shannonEntropy, _shannonEntropy);

        }

        [Test]
        public void SelectBestSplit_CategoricalAttributes_CategoricalOutput()
        {
            // Given
            var testData = TestDataBuilder.ReadWeatherDataWithCategoricalAttributes();
            
            // When
            var bestSplit = _binaryBestSplitSelector.SelectBestSplit(testData, "Play", _categoricalSplitQualityChecker);

            // Then
            Assert.IsNotNull(bestSplit);
        }

        [Test]
        public void SelectBestSplit_NumericAttributes_CategoricalOutput()
        {
            // Given
            var testData = TestDataBuilder.ReadWeatherDataWithMixedAttributes();
            var subject = new BinarySplitSelector<object>(new BinaryDiscreteDataSplitter<object>(),
                _binaryNumericDataSplitter);

            // When
            var bestSplit = subject.SelectBestSplit(testData, "Play", _categoricalSplitQualityChecker);

        }
    }
}
