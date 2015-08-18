using System.Collections.Generic;
using BrainSharper.Abstract.Algorithms.DecisionTrees.Processors;
using BrainSharper.Abstract.MathUtils.ImpurityMeasures;
using BrainSharper.Implementations.Algorithms.DecisionTrees.Processors;
using BrainSharper.Implementations.MathUtils.ImpurityMeasures;
using BrainSharperTests.TestUtils;
using NUnit.Framework;

namespace BrainSharperTests.Implementations.Algorithms.DecisionTrees.Processors
{
    using System.Data;

    using BrainSharper.Abstract.Algorithms.DecisionTrees.DataStructures;
    using BrainSharper.Implementations.Data;

    [TestFixture]
    public class BestSplitSelectorsTests
    {
        private readonly IBinaryNumericDataSplitter binaryNumericDataSplitter = new BinaryNumericDataSplitter();
        private readonly IDataSplitter multiValueCategoricalDataSplitter;

        private readonly IBinaryBestSplitSelector binaryBestSplitSelector;
        private readonly IBestSplitSelector multiValueBestSplitSelector;

        private readonly IBinaryNumericAttributeSplitPointSelector binaryNumericBestSplitPointSelector;
        private readonly IBinaryNumericAttributeSplitPointSelector dynamicProgrammingBestNumericSplitFinder;

        private readonly ISplitQualityChecker categoricalBinarySplitQualityChecker;
        private readonly ISplitQualityChecker categoricalMultiValueSplitQualityChecker;

        

        public BestSplitSelectorsTests()
        {
            ICategoricalImpurityMeasure<string> shannonEntropy = new ShannonEntropy<string>();
            IBinaryDataSplitter binaryDataSplitter = new BinaryDiscreteDataSplitter();
            binaryNumericBestSplitPointSelector = new ClassBreakpointsNumericSplitFinder();
            binaryBestSplitSelector = new BinarySplitSelectorForCategoricalOutcome(binaryDataSplitter, binaryNumericDataSplitter, binaryNumericBestSplitPointSelector);
            categoricalBinarySplitQualityChecker = new InformationGainCalculator<string>(shannonEntropy, shannonEntropy);
            categoricalMultiValueSplitQualityChecker = new InformationGainCalculator<string>(shannonEntropy, shannonEntropy);
            multiValueCategoricalDataSplitter = new MultiValueDiscreteDataSplitter();
            multiValueBestSplitSelector = new MultiValueSplitSelectorForCategoricalOutcome(multiValueCategoricalDataSplitter, binaryNumericDataSplitter, binaryNumericBestSplitPointSelector);
            dynamicProgrammingBestNumericSplitFinder = new DynamicProgrammingNumericSplitFinder();
        }

        [Test]
        public void SelectBestSplitBinarySplitCategoricalAttributesCategoricalOutput()
        {
            // Given
            var testData = TestDataBuilder.ReadWeatherDataWithCategoricalAttributes();

            // When
            var bestSplit = this.binaryBestSplitSelector.SelectBestSplit(testData, "Play", this.categoricalBinarySplitQualityChecker, new AlreadyUsedAttributesInfo());

            // Then
            Assert.IsNotNull(bestSplit);
        }

        [Test]
        public void SelectBestSplitBinarySplitNumericAttributesCategoricalOutput()
        {
            // Given
            var testData = TestDataBuilder.ReadWeatherDataWithMixedAttributes().GetSubsetByColumns(new[] { "Temperature", "Humidity", "Windy", "Play" });
            var subject = new BinarySplitSelectorForCategoricalOutcome(new BinaryDiscreteDataSplitter(), this.binaryNumericDataSplitter, this.binaryNumericBestSplitPointSelector);

            // When
            var bestSplit = subject.SelectBestSplit(testData, "Play", this.categoricalBinarySplitQualityChecker, new AlreadyUsedAttributesInfo());
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
            var bestSplit = this.multiValueBestSplitSelector.SelectBestSplit(testData, "Play", this.categoricalMultiValueSplitQualityChecker, new AlreadyUsedAttributesInfo());

            // Then
            Assert.AreEqual(expectedBestSplitAttribute, bestSplit.SplittingFeatureName);
            Assert.AreEqual(expectedRowCountsPerAttribute.Count, bestSplit.SplittedDataSets.Count);
            foreach (var splittedData in bestSplit.SplittedDataSets)
            {
                var expectedRowCounts = expectedRowCountsPerAttribute[splittedData.SplitLink.TestResult];
                Assert.AreEqual(expectedRowCounts, splittedData.SplittedDataFrame.RowCount);
            }
        }

        [Test]
        public void SelectBestNumericSplitPoint_CategoricalVariable()
        {
            //Given
            var weatherDataNumeric = TestDataBuilder.ReadWeatherDataWithMixedAttributes();
            var initialEntropy = categoricalBinarySplitQualityChecker.GetInitialEntropy(weatherDataNumeric, "Play");

            // When
            var bruteForceBestSplit = binaryNumericBestSplitPointSelector.FindBestSplitPoint(
                weatherDataNumeric,
                "Play",
                "Temperature",
                categoricalBinarySplitQualityChecker,
                binaryNumericDataSplitter,
                initialEntropy);
            var dynamicProgrammingBestSplit = dynamicProgrammingBestNumericSplitFinder.FindBestSplitPoint(
                weatherDataNumeric,
                "Play",
                "Temperature",
                categoricalBinarySplitQualityChecker,
                binaryNumericDataSplitter,
                initialEntropy);

            // Then
            Assert.AreEqual(bruteForceBestSplit.Item1.SplittingFeatureName, dynamicProgrammingBestSplit.Item1.SplittingFeatureName);
            Assert.AreEqual((bruteForceBestSplit.Item1 as IBinarySplittingResult).SplittingValue, (dynamicProgrammingBestSplit.Item1 as IBinarySplittingResult).SplittingValue);
            Assert.AreEqual(bruteForceBestSplit.Item2, dynamicProgrammingBestSplit.Item2);
        }

        [Test]
        public void SelectBestNumericSplitPoint_NumericalFeature_DummyDataSet()
        {
            // Given
            var data = new DataTable
                           {
                               Columns =
                                   {
                                       new DataColumn("Col1", typeof(double)),
                                       new DataColumn("ValueCol", typeof(double))
                                   },
                               Rows =
                                   {
                                       { 1.0, 10 },
                                       { 2.0, 11.0 },
                                       { 3.0, 9.0 },
                                       { 4.0, 20.0 },
                                       { 5.0, 21.0 },
                                       { 6.0, 22.0 }
                                   }
                           };
            var dataFrame = new DataFrame(data);
            var splitQualityMeasure = new VarianceBasedSplitQualityChecker();
            var subject = new BestSplitSelectorForNumericValues(binaryNumericDataSplitter);

            // When
            var bestSplit = subject.SelectBestSplit(
                dataFrame,
                "ValueCol",
                splitQualityMeasure,
                new AlreadyUsedAttributesInfo()) as IBinarySplittingResult;

            // Then
            Assert.AreEqual(3.5, bestSplit.SplittingValue);
        }
    }
}
