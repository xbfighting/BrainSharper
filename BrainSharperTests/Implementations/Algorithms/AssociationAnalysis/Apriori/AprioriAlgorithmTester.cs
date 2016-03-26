using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using BrainSharper.Abstract.Algorithms.AssociationAnalysis.DataStructures;
using BrainSharper.Abstract.Data;
using BrainSharper.Implementations.Algorithms.AssociationAnalysis.Apriori;
using BrainSharper.Implementations.Algorithms.AssociationAnalysis.DataStructures.Common;
using BrainSharper.Implementations.Data;
using BrainSharperTests.TestUtils;
using MathNet.Numerics.Statistics;
using NUnit.Framework;
using static BrainSharperTests.Implementations.Algorithms.AssociationAnalysis.AssociationAnalysisTestDataBuilder;

namespace BrainSharperTests.Implementations.Algorithms.AssociationAnalysis.Apriori
{
    [TestFixture]
    public class AprioriAlgorithmTester
    {
        public static readonly ITransactionsSet<IDataItem<string>> MarketBasketTransactions =
            MarketBasketDataSet1.ToAssociativeTransactionsSet<string>(TranId);

        public static AprioriAlgorithm<IDataItem<string>> Subject = new AprioriAlgorithm<IDataItem<string>>();

        [Test]
        public void GenerateInitialFrequentItems()
        {
            // Given
            var miningParams = new FrequentItemsMiningParams(0.3, 0.7);
            var expectedInitialItems = new List<IFrequentItemsSet<IDataItem<string>>>
                                           {
                                               new FrequentItemsSet<IDataItem<string>>(
                                                    3,
                                                    0.6,
                                                    new object[] { "2", "4", "5" },
                                                    new DataItem<string>(
                                                        Product,
                                                        Beer)
                                                ),
                                               new FrequentItemsSet<IDataItem<string>>(
                                                    2,
                                                    0.4,
                                                    new object[] { "2", "5" },
                                                    new DataItem<string>(
                                                        Product,
                                                        Diapers)
                                                ),
                                               new FrequentItemsSet<IDataItem<string>>(
                                                    4,
                                                    0.8,
                                                    new object[] { "1", "2", "4", "5" },
                                                    new DataItem<string>(
                                                        Product,
                                                        Nuts)
                                                ),
                                               new FrequentItemsSet<IDataItem<string>>(
                                                    3,
                                                    0.6,
                                                    new object[] { "1", "3", "4" },
                                                    new DataItem<string>(
                                                        Product,
                                                        CocaCola)
                                                ),
                                           };


            // When
            IList<IFrequentItemsSet<IDataItem<string>>> actualInitialItems = Subject.GenerateInitialItemsSet(MarketBasketTransactions, miningParams);

            // Then
            CollectionAssert.AreEquivalent(expectedInitialItems, actualInitialItems);
        }

        [Test]
        public void TestGenerateNextItemsSetOfSize2()
        {
            // Given
            var miningParams = new FrequentItemsMiningParams(0.3, 0.7);
            var frequentItemsetsOfSize2 = new List<IFrequentItemsSet<IDataItem<string>>>
            {
                new FrequentItemsSet<IDataItem<string>>(2, 0.4, new object[] { "2", "5" }, new DataItem<string>(Product, Beer), new DataItem<string>(Product, Diapers)),
                new FrequentItemsSet<IDataItem<string>>(3, 0.6, new object[] { "2", "4", "5" }, new DataItem<string>(Product, Beer), new DataItem<string>(Product, Nuts)),
                new FrequentItemsSet<IDataItem<string>>(2, 0.4, new object[] { "2", "5" }, new DataItem<string>(Product, Diapers), new DataItem<string>(Product, Nuts)),
                new FrequentItemsSet<IDataItem<string>>(2, 0.4, new object[] { "1", "4" }, new DataItem<string>(Product, Nuts), new DataItem<string>(Product, CocaCola)),
            };

            // When
            var initialItemSets = Subject.GenerateInitialItemsSet(MarketBasketTransactions, miningParams);
            var candidateItemsets =
                Subject.GenerateNextItems(MarketBasketTransactions, miningParams, new Dictionary<int, IList<IFrequentItemsSet<IDataItem<string>>>> { [1] =  initialItemSets }, 2);
            var candidateItemsetsMeetingCriteria = candidateItemsets.Where(itms => itms.MeetsCriteria).Select(itm => itm.FrequentItemsSet).ToList();

            // Then
            Assert.AreEqual(4, candidateItemsetsMeetingCriteria.Count);
            CollectionAssert.AreEquivalent(frequentItemsetsOfSize2, candidateItemsetsMeetingCriteria);
        }

        [Test]
        public void SelectOnlyFrequentItemsSetsHeuristic()
        {
            // Given
            var miningParams = new FrequentItemsMiningParams(0.3, 0.7);
            var frequentItemsetsOfSize2 = new List<IFrequentItemsSet<IDataItem<string>>>
            {
                new FrequentItemsSet<IDataItem<string>>(2, 0.4, new object[] { 2, 5 }, new DataItem<string>(Product, Beer), new DataItem<string>(Product, Diapers)),
                new FrequentItemsSet<IDataItem<string>>(3, 0.6, new object[] { 2, 4, 5 }, new DataItem<string>(Product, Beer), new DataItem<string>(Product, Nuts)),
                new FrequentItemsSet<IDataItem<string>>(2, 0.4, new object[] { 2, 5 }, new DataItem<string>(Product, Diapers), new DataItem<string>(Product, Nuts)),
                new FrequentItemsSet<IDataItem<string>>(2, 0.4, new object[] { 2, 5 }, new DataItem<string>(Product, Nuts), new DataItem<string>(Product, CocaCola)),
                new FrequentItemsSet<IDataItem<string>>(1, 0.2, new object[] { 4 }, new DataItem<string>(Product, Beer), new DataItem<string>(Product, CocaCola)),
                new FrequentItemsSet<IDataItem<string>>(0, 0.0, new object[0], new DataItem<string>(Product, Diapers), new DataItem<string>(Product, CocaCola))
            };

            // When
            var candidateItemsOfSize3 = Subject.GenerateNextItems(MarketBasketTransactions, miningParams, new Dictionary<int, IList<IFrequentItemsSet<IDataItem<string>>>> { [2] = frequentItemsetsOfSize2 }, 3);

            // Then
            Assert.AreEqual(4, candidateItemsOfSize3.Count);
        }

        [Test]
        public void TestFindFrequentItems()
        {
            // Given
            var miningParams = new FrequentItemsMiningParams(0.3, 0.7);
            var expectedFrequentItems = new List<IFrequentItemsSet<IDataItem<string>>>
                                           {
                                               new FrequentItemsSet<IDataItem<string>>(3, 0.6, new object[] { "2", "4", "5" }, new DataItem<string>(Product, Beer)),
                                               new FrequentItemsSet<IDataItem<string>>(2, 0.4, new object[] { "2", "5" }, new DataItem<string>(Product, Diapers)),
                                               new FrequentItemsSet<IDataItem<string>>(4, 0.8, new object[] { "1", "2", "4", "5" }, new DataItem<string>(Product, Nuts)),
                                               new FrequentItemsSet<IDataItem<string>>(3, 0.6, new object[] { "1", "3", "4" }, new DataItem<string>(Product, CocaCola)),
                                               new FrequentItemsSet<IDataItem<string>>(2, 0.4, new object[] { "2", "5" }, new DataItem<string>(Product, Beer), new DataItem<string>(Product, Diapers)),
                                               new FrequentItemsSet<IDataItem<string>>(3, 0.6, new object[] { "2", "4", "5" }, new DataItem<string>(Product, Beer), new DataItem<string>(Product, Nuts)),
                                               new FrequentItemsSet<IDataItem<string>>(2, 0.4, new object[] { "2", "5" }, new DataItem<string>(Product, Diapers), new DataItem<string>(Product, Nuts)),
                                               new FrequentItemsSet<IDataItem<string>>(2, 0.4, new object[] { "1", "4" }, new DataItem<string>(Product, Nuts), new DataItem<string>(Product, CocaCola)),
                                               new FrequentItemsSet<IDataItem<string>>(2, 0.4, new object[] { "2", "5" }, new DataItem<string>(Product, Nuts), new DataItem<string>(Product, Diapers), new DataItem<string>(Product, Beer))

                                           };

            // When
            var frequentItems = Subject.FindFrequentItems(MarketBasketTransactions, miningParams);

            // Then
            CollectionAssert.AreEquivalent(expectedFrequentItems, frequentItems.FrequentItems);
        }

        [Test]
        public void TestBuildingAssociationRules_SingleElementConsequent()
        {
            // Given
            var itemsMiningParams = new FrequentItemsMiningParams(0.3, 0.7);
            var rulesMiningParams = new AssociationMiningParams(0.3, 0.6);

            // When
            var frequentItems = Subject.FindFrequentItems(MarketBasketTransactions, itemsMiningParams);
            var assocRules = Subject.FindAssociationRules(MarketBasketTransactions, frequentItems, rulesMiningParams);

            // Then
            Assert.AreEqual(9, assocRules.Count);
        }

        [Test]
        public void TestBuildingAssociationRules_MultiElementConsequent()
        {
            // Given
            var itemsMiningParams = new FrequentItemsMiningParams(0.3, 0.7);
            var rulesMiningParams = new AssociationMiningParams(0.3, 0.6, allowMultiSelectorConsequent: true);

            // When
            var frequentItems = Subject.FindFrequentItems(MarketBasketTransactions, itemsMiningParams);
            var assocRules = Subject.FindAssociationRules(MarketBasketTransactions, frequentItems, rulesMiningParams);

            // Then
            Assert.AreEqual(11, assocRules.Count);
        }

        [Test]
        public void TestBuildingAssociationRules_BigDataSetCongress()
        {
            // Given
            var dataSet = TestDataBuilder.ReadCongressData()
                .ToAssociativeTransactionsSet<string>();
            var rulesMiningParams = new AssociationMiningParams(0.2, 0.8);
            var expectedItemsCount = 13932;

            // When
            var frequentItems = Subject.FindFrequentItems(dataSet, rulesMiningParams);

            // Then
            Assert.AreEqual(expectedItemsCount, frequentItems.FrequentItems.Count);
        }

        [Test]
        public void TestBuildingAssociationRules_BigDataSetCongress_PerformanceMeasures()
        {
            /*
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
                sw.Stop();
                executionTimes.Add(sw.ElapsedMilliseconds);
            }

            // Then
            var avgExecutionTime = executionTimes.Average();
            var std = ArrayStatistics.MeanStandardDeviation(executionTimes.ToArray());
            var medianExecTime = ArrayStatistics.MedianInplace(executionTimes.ToArray());
            */
        }

        [Test]
        public void TestAprioriOnMushroomData()
        {
            // Given
            var data = new TransactionsSet<IDataItem<string>>(
                TestDataBuilder.ReadMushroomDataWithCategoricalAttributes()
                .ToAssociativeTransactionsSet<string>()
                .TransactionsList
                );
            var miningParams = new FrequentItemsMiningParams(0.5, 0.9);
            var expectedCount = 153;
            
            // When
            var results = Subject.FindFrequentItems(data, miningParams);

            // Then
            Assert.AreEqual(expectedCount, results.FrequentItems.Count);
        }
    }
}
