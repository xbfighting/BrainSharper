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
using BrainSharper.Implementations.Algorithms.AssociationAnalysis.DissociationRules;
using BrainSharper;

namespace BrainSharperTests.Implementations.Algorithms.AssociationAnalysis.DissociationRules
{
    [TestFixture]
    public class DissociationRulesFinderTester
    {
        public static readonly ITransactionsSet<IDataItem<string>> MarketBasketTransactions =
            MarketBasketDataSet1.ToAssociativeTransactionsSet<string>(TranId);

        [Test]
        public void FindValidAndCandidateDissociationItems()
        {
            // Given
            var subject = new DissociationRulesFinder<string>();
            ITransactionsSet<string> abstractSet = AbstractTransactionsSet;
            var miningParams = new DisociativeRulesMiningParams(0.4, maxRelativeJoin: 0.1, minimalConfidence: 1.0);
			var expectedValidDissociativeItems = new Dictionary<int, IList<IDissociativeRuleCandidateItemset<string>>>
            {
				[2] = new List<IDissociativeRuleCandidateItemset<string>>
                {
					new DissociativeRuleCandidateItemSet<string>(
						new FrequentItemsSet<string>(new HashSet<string> { "C" }, 2, 0.4),
						new FrequentItemsSet<string>(new HashSet<string> { "E" }, 2, 0.4),
						0, 0
					),
					new DissociativeRuleCandidateItemSet<string>(
						new FrequentItemsSet<string>(new HashSet<string> { "C" }, 2, 0.4),
						new FrequentItemsSet<string>(new HashSet<string> { "D" }, 3, 0.6),
						0, 0
					)
                }
            };

			var expectedCandidateDissociativeItems = new Dictionary<int, IList<IDissociativeRuleCandidateItemset<string>>>
			{
				[2] = new List<IDissociativeRuleCandidateItemset<string>>
				{

					new DissociativeRuleCandidateItemSet<string>(
						new FrequentItemsSet<string>(new HashSet<string> { "B" }, 4, 0.8),
						new FrequentItemsSet<string>(new HashSet<string> { "E" }, 2, 0.4)
					),

					new DissociativeRuleCandidateItemSet<string>(
						new FrequentItemsSet<string>(new HashSet<string> { "A" }, 3, 0.6),
						new FrequentItemsSet<string>(new HashSet<string> { "E" }, 2, 0.4)
					),

					new DissociativeRuleCandidateItemSet<string>(
						new FrequentItemsSet<string>(new HashSet<string> { "A" }, 3, 0.6),
						new FrequentItemsSet<string>(new HashSet<string> { "C" }, 2, 0.4)
					)
				},
				[3] = new List<IDissociativeRuleCandidateItemset<string>>
				{
					new DissociativeRuleCandidateItemSet<string>(
						new FrequentItemsSet<string>(new HashSet<string> { "B" }, 4, 0.8),
						new FrequentItemsSet<string>(new HashSet<string> { "E" }, 2, 0.4)
					)
				}
			};

            // When
            IFrequentItemsWithDissociativeSets<string> results = subject.FindFrequentItems(abstractSet, miningParams) as IFrequentItemsWithDissociativeSets<string>;

            // Then
			var validDissociativeSetsBySize = results.ValidDissociativeSets
				.GroupBy (itm => itm.AllItemsSet.Count)
				.ToDictionary (grp => grp.Key, grp => grp);
			
			var itemssize2 = validDissociativeSetsBySize [2];
			var expectedItemsSize2 = expectedValidDissociativeItems [2];
			Assert.AreEqual (itemssize2.First (), expectedItemsSize2.First ());


			CollectionAssert.AreEquivalent(validDissociativeSetsBySize[2], expectedValidDissociativeItems[2]);
			CollectionAssert.AreEquivalent(validDissociativeSetsBySize[3], expectedValidDissociativeItems[3]);
        }

        [Test]
        public void FindNegativeBoundaryOnLargerDataset()
        {
            // Given
            var data = TestDataBuilder.ReadCongressData().ToAssociativeTransactionsSet<string>();
            var subject = new DissociationRulesFinder<IDataItem<string>>();
            var miningParams = new DisociativeRulesMiningParams(0.4, maxRelativeJoin: 0.1, minimalConfidence: 1.0);

            // When
            var results = subject.FindFrequentItems(data, miningParams) as IFrequentItemsWithDissociativeSets<IDataItem<string>>;

            // Then
            Assert.IsNotNull(results.ValidDissociativeSets);
        }
    }
}
