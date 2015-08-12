namespace BrainSharperTests.Implementations.Algorithms.DecisionTrees.Processors
{
    #region

    using System.Collections.Generic;
    using System.Data;
    using System.Linq;

    using BrainSharper.Abstract.Algorithms.DecisionTrees.DataStructures;
    using BrainSharper.Implementations.Algorithms.DecisionTrees.DataStructures;
    using BrainSharper.Implementations.Algorithms.DecisionTrees.Processors;
    using BrainSharper.Implementations.Data;
    using BrainSharper.Implementations.MathUtils.ImpurityMeasures;

    using BrainSharperTests.TestUtils;

    using NUnit.Framework;

    #endregion

    [TestFixture]
    public class SplitQualityCheckersTests
    {
        private readonly ShannonEntropy<string> shannonEntropy = new ShannonEntropy<string>();

        private readonly InformationGainCalculator<string> informationGainCalculator;

        private readonly InformationGainCalculator<string> informationGainRatioCalculator;

        public SplitQualityCheckersTests()
        {
            this.informationGainCalculator = new InformationGainCalculator<string>(this.shannonEntropy, this.shannonEntropy);
            this.informationGainRatioCalculator = new InformationGainRatioCalculator<string>(
                this.shannonEntropy, 
                this.shannonEntropy);
        }

        [Test]
        public void TestInformationGainCalculatorMultipleSplitByOvercast()
        {
            // Given
            var initialData = TestDataBuilder.ReadWeatherDataWithCategoricalAttributes();
            var subset1 = initialData.GetSubsetByRows(new[] { 0, 1, 7, 8, 10 });
            var subset2 = initialData.GetSubsetByRows(new[] { 2, 6, 11, 12 });
            var subset3 = initialData.GetSubsetByRows(new[] { 3, 4, 5, 9, 13 });

            var splitResults = new List<ISplittedData>
                                   {
                                       new SplittedData(
                                           new DecisionLink(0, 0, "sunny"), 
                                           subset1), 
                                       new SplittedData(
                                           new DecisionLink(0, 0, "overcast"), 
                                           subset2), 
                                       new SplittedData(
                                           new DecisionLink(0, 0, "rainy"), 
                                           subset3)
                                   };

            var expectedGain = 0.246;

            // When
            var actualGain = this.informationGainCalculator.CalculateSplitQuality(
                initialData, 
                splitResults, 
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

            var splitResults = new List<ISplittedData>
                                   {
                                       new SplittedData(
                                           new DecisionLink(0, 0, "sunny"), 
                                           subset1), 
                                       new SplittedData(
                                           new DecisionLink(0, 0, "overcast"), 
                                           subset2), 
                                       new SplittedData(
                                           new DecisionLink(0, 0, "rainy"), 
                                           subset3)
                                   };

            var expectedGain = 0.156;

            // When
            var actualGain = informationGainRatioCalculator.CalculateSplitQuality(
                initialData, 
                splitResults, 
                TestDataBuilder.WeatherDataDependentFeatureName);

            // Then
            Assert.AreEqual(expectedGain, actualGain, 0.0009);
        }

        [Test]
        public void TestInformationGainCalculator_WithGroupsCountsOnly()
        {
            // Given
            var initialDataTable = new DataFrame(
                                      new DataTable()
                                       {
                                           Columns = { new DataColumn("Col1", typeof(string)) },
                                           Rows =
                                               {
                                                    new object[] { "A" },
                                                    new object[] { "A" },
                                                    new object[] { "A" },
                                                    new object[] { "B" },
                                                    new object[] { "B" },
                                               }
                                       });
            var group1 = initialDataTable.GetSubsetByRows(new[] { 0, 1, 3 });
            var splittedDataGroup1 = new SplittedData(null, group1);
            var group1UniqueValuesCount =
                group1.GetColumnVector("Col1")
                    .Values.GroupBy(val => val, val => val)
                    .Select(grp => grp.Count())
                    .ToList();

            var group2 = initialDataTable.GetSubsetByRows(new[] { 2, 4 });
            var splittedDataGroup2 = new SplittedData(null, group2);
            var group2UniqueValuesCount =
                group2.GetColumnVector("Col1")
                    .Values.GroupBy(val => val, val => val)
                    .Select(grp => grp.Count())
                    .ToList();

            var groupsCounts = new List<IList<int>> { group1UniqueValuesCount, group2UniqueValuesCount };

            // When
            var entropyFromGroups = informationGainCalculator.CalculateSplitQuality(
                1.0,
                5,
                new List<ISplittedData> { splittedDataGroup1, splittedDataGroup2 },
                "Col1");

            var entropyFromGroupCounts = informationGainCalculator.CalculateSplitQuality(1.0, 5, groupsCounts);
            
            // Then
            Assert.AreEqual(entropyFromGroupCounts, entropyFromGroups);
        }
    }
}