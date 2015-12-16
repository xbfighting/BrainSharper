namespace BrainSharperTests.Implementations.Algorithms.AssociationAnalysis.Apriori
{
    using System.Collections.Generic;

    using BrainSharper.Abstract.Algorithms.AssociationAnalysis.DataStructures;
    using BrainSharper.Abstract.Data;
    using BrainSharper.Implementations.Algorithms.AssociationAnalysis.Apriori;
    using BrainSharper.Implementations.Algorithms.AssociationAnalysis.DataStructures;
    using BrainSharper.Implementations.Data;

    using static AssociationAnalysisTestDataBuilder;
    using NUnit.Framework;

    [TestFixture]
    public class AprioriAlgorithmTester
    {
        public static readonly ITransactionsSet<IDataItem<string>> MarketBasketTransactions =
            MarketBasketDataSet1.ToAssociativeTransactionsSet<string>(TranId);

        public static AprioriAlgorithm<IDataItem<string>> Subject = new AprioriAlgorithm<IDataItem<string>>();

        [Test]
        public void GenerateInitialCandidateItems()
        {
            // Given
            var miningParams = new AssociationMiningParams(0.3, 0.7);
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
        public void GenerateCandidateItemsetsOfSize2()
        {
            // Given
            // Given
            var miningParams = new AssociationMiningParams(0.3, 0.7);
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
            var initialItemSets = Subject.GenerateInitialItemsSet(MarketBasketTransactions, miningParams);
            IList<IFrequentItemsSet<IDataItem<string>>> candidateItemsets =
                Subject.GenerteCandidateItems(MarketBasketTransactions, miningParams, initialItemSets, 2);

            // Then
            Assert.AreEqual(6, candidateItemsets.Count);
            Assert.IsNotNull(candidateItemsets);
        }
    }
}
