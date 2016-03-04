using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BrainSharper.Abstract.Algorithms.AssociationAnalysis.DataStructures;
using BrainSharper.Abstract.Data;
using BrainSharper.Implementations.Algorithms.AssociationAnalysis.DataStructures.Common;
using BrainSharper.Implementations.Algorithms.AssociationAnalysis.FPGrowth;
using BrainSharper.Implementations.Data;
using BrainSharperTests.TestUtils;
using MathNet.Numerics.Statistics;
using NUnit.Framework;
using static BrainSharperTests.Implementations.Algorithms.AssociationAnalysis.AssociationAnalysisTestDataBuilder;


namespace BrainSharperTests.Implementations.Algorithms.AssociationAnalysis.FpGrowth
{
    [TestFixture]
    public class FpGrowthTests
    {
        protected readonly FpGrowthBuilder<IDataItem<string>> Subject = new FpGrowthBuilder<IDataItem<string>>();

        [Test]
        public void TestBuildingFpTree()
        {
            // Given
            var data = new TransactionsSet<IDataItem<string>>(
                MarketBasketDataSet1
                .ToAssociativeTransactionsSet<string>(TranId)
                .TransactionsList
                .OrderBy(tran => tran.TransactionKey)
                );
            var expectedFrequentItems = new List<IFrequentItemsSet<IDataItem<string>>>
                                           {
                                               new FrequentItemsSet<IDataItem<string>>(3, 0.6, new string[] { "2", "4", "5" }, new DataItem<string>(Product, Beer)),
                                               new FrequentItemsSet<IDataItem<string>>(2, 0.4, new string[] { "2", "5" }, new DataItem<string>(Product, Diapers)),
                                               new FrequentItemsSet<IDataItem<string>>(4, 0.8, new string[] { "1", "2", "4", "5" }, new DataItem<string>(Product, Nuts)),
                                               new FrequentItemsSet<IDataItem<string>>(3, 0.6, new string[] { "1", "3", "4" }, new DataItem<string>(Product, CocaCola)),
                                               new FrequentItemsSet<IDataItem<string>>(2, 0.4, new DataItem<string>(Product, Beer), new DataItem<string>(Product, Diapers)),
                                               new FrequentItemsSet<IDataItem<string>>(3, 0.6, new DataItem<string>(Product, Beer), new DataItem<string>(Product, Nuts)),
                                               new FrequentItemsSet<IDataItem<string>>(2, 0.4, new DataItem<string>(Product, Diapers), new DataItem<string>(Product, Nuts)),
                                               new FrequentItemsSet<IDataItem<string>>(2, 0.4, new DataItem<string>(Product, Nuts), new DataItem<string>(Product, CocaCola)),
                                               new FrequentItemsSet<IDataItem<string>>(2, 0.4, new DataItem<string>(Product, Nuts), new DataItem<string>(Product, Diapers), new DataItem<string>(Product, Beer))

                                           };

            var miningParams = new FrequentItemsMiningParams(0.3, 0.7);

            // When
            var actualFrequentItems = Subject.FindFrequentItems(data, miningParams);

            // Then
            Assert.AreEqual(9, actualFrequentItems.FrequentItems.Count);
            CollectionAssert.AreEquivalent(expectedFrequentItems, actualFrequentItems.FrequentItems);
        }

        [Test]
        public void Test_BuildingFpTree_LargeDataset_PerformanceMeasures()
        {
            // Given
            var dataSet = TestDataBuilder.ReadCongressData()
                .ToAssociativeTransactionsSet<string>();
            var rulesMiningParams = new AssociationMiningParams(0.2, 0.8);

            // When
            var executionTimes = new List<double>();

            for (int i = 0; i < 10; i++)
            {
                GC.Collect();
                var sw = Stopwatch.StartNew();
                var frequentItems = Subject.FindFrequentItems(dataSet, rulesMiningParams);
                var groupedItems = frequentItems.FrequentItemsBySize[2]
                    .GroupBy(itm => itm)
                    .ToDictionary(grp => grp.Key, grp => grp.Count());

                var uniqueFrequentItemsCount = frequentItems.FrequentItems.Distinct().Count();
                Assert.AreEqual(frequentItems.FrequentItems.Count(), uniqueFrequentItemsCount);
                
                sw.Stop();
                executionTimes.Add(sw.ElapsedMilliseconds);
            }

            // Then
            var avgExecutionTime = executionTimes.Average();
            var std = ArrayStatistics.MeanStandardDeviation(executionTimes.ToArray());
            var medianExecTime = ArrayStatistics.MedianInplace(executionTimes.ToArray());
        }

    }
}