using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using BrainSharper.Abstract.Algorithms.AssociationAnalysis.DataStructures;
using BrainSharper.Abstract.Data;
using BrainSharper.Implementations.Algorithms.AssociationAnalysis.Apriori;
using BrainSharper.Implementations.Algorithms.AssociationAnalysis.DataStructures;
using BrainSharper.Implementations.Data;
using BrainSharperTests.TestUtils;
using static BrainSharperTests.Implementations.Algorithms.AssociationAnalysis.AssociationAnalysisTestDataBuilder;

using NUnit.Framework;

namespace BrainSharperTests.Implementations.Algorithms.AssociationAnalysis.Apriori
{
    [TestFixture]
    public class DissociationRulesFinderTester
    {
        public static readonly ITransactionsSet<IDataItem<string>> MarketBasketTransactions =
            MarketBasketDataSet1.ToAssociativeTransactionsSet<string>(TranId);

        [Test]
        public void FindNgativeBoundaryTest()
        {
            // Given
            var subject = new DissociationRulesFinder<string>();
            ITransactionsSet<string> abstractSet = AbstractTransactionsSet;
            var miningParams = new AssociationMiningParams(0.4, 1.0);
            var expectedNegativeBoundaryItems = new Dictionary<int, IList<IFrequentItemsSet<string>>>
            {
                [2] = new List<IFrequentItemsSet<string>>
                {
                    new FrequentItemsSet<string>(new HashSet<string> {"C", "E"}, 0.0),
                    new FrequentItemsSet<string>(new HashSet<string> {"C", "D"}, 0.0),
                    new FrequentItemsSet<string>(new object[] { 4 }, new HashSet<string> {"B", "E"}, 1, 0.2),
                    new FrequentItemsSet<string>(new object[] { 3 }, new HashSet<string> {"A", "E"}, 1, 0.2),
                    new FrequentItemsSet<string>(new object[] { 5 }, new HashSet<string> {"A", "C"}, 1, 0.2)
                },
                [3] = new List<IFrequentItemsSet<string>>
                {
                    new FrequentItemsSet<string>(new object[] { 1 }, new HashSet<string> {"A", "B", "D"}, 1, 0.2)
                }
            };

            // When
            var results = subject.FindFrequentItems(abstractSet, miningParams) as IFrequentItemsWithNegativeboundarySearchResult<string>;

            // Then
            CollectionAssert.AreEquivalent(results.GetNegativeBoundaryItemsBySize(2), expectedNegativeBoundaryItems[2]);
            CollectionAssert.AreEquivalent(results.GetNegativeBoundaryItemsBySize(3), expectedNegativeBoundaryItems[3]);
        }

        [Test]
        public void FindNegativeBoundaryOnLargerDataset()
        {
            // Given
            var data = TestDataBuilder.ReadCongressData().ToAssociativeTransactionsSet<string>();
            var subject = new DissociationRulesFinder<IDataItem<string>>();
            var miningParams = new AssociationMiningParams(0.2, 0.9);

            // When
            var results = subject.FindFrequentItems(data, miningParams) as IFrequentItemsWithNegativeboundarySearchResult<IDataItem<string>>;

            // Then
            Assert.IsNotNull(results.NegativeBoundaryItems);
        }
    }
}
