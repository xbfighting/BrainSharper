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
using BrainSharper.Abstract.Algorithms.AssociationAnalysis.DissociativeRulesMining;

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
            new FrequentItemsSet<string>(new HashSet<string> { "A" }, 3, 0.6),
            new FrequentItemsSet<string>(new HashSet<string> { "E" }, 2, 0.4),
            1, 0.2
          ),
          new DissociativeRuleCandidateItemSet<string>(
            new FrequentItemsSet<string>(new HashSet<string> { "C" }, 2, 0.4),
            new FrequentItemsSet<string>(new HashSet<string> { "A" }, 3, 0.6),
            1, 0.2
          ),
          new DissociativeRuleCandidateItemSet<string>(
            new FrequentItemsSet<string>(new HashSet<string> { "E" }, 2, 0.4),
            new FrequentItemsSet<string>(new HashSet<string> { "B" }, 4, 0.8),
            1, 0.2
          )
        },
                [3] = new List<IDissociativeRuleCandidateItemset<string>>
        {
          new DissociativeRuleCandidateItemSet<string>(
            new FrequentItemsSet<string>(new HashSet<string> { "B" }, 4, 0.8),
            new FrequentItemsSet<string>(new HashSet<string> { "A", "D" }, 1, 0.2),
            1, 0.2
          ),
          new DissociativeRuleCandidateItemSet<string>(
            new FrequentItemsSet<string>(new HashSet<string> { "D" }, 3, 0.6),
            new FrequentItemsSet<string>(new HashSet<string> { "A", "B" }, 2, 0.4),
            1, 0.2
          ),
          new DissociativeRuleCandidateItemSet<string>(
            new FrequentItemsSet<string>(new HashSet<string> { "A" }, 3, 0.6),
            new FrequentItemsSet<string>(new HashSet<string> { "B", "D" }, 2, 0.4),
            1, 0.2
          )
        }
            };

            // When
            IFrequentItemsWithDissociativeSets<string> results = subject.FindFrequentItems(abstractSet, miningParams) as IFrequentItemsWithDissociativeSets<string>;

            // Then
            var validDissociativeSetsBySize = results.ValidDissociativeSets
              .GroupBy(itm => itm.AllItemsSet.Count)
              .ToDictionary(grp => grp.Key, grp => grp);

            var candidateDissociativeSetsBySize = results.CandidateDissociativeSets
              .GroupBy(itm => itm.AllItemsSet.Count)
              .ToDictionary(grp => grp.Key, grp => grp);

            CollectionAssert.AreEquivalent(validDissociativeSetsBySize[2], expectedValidDissociativeItems[2]);
            CollectionAssert.AreEquivalent(candidateDissociativeSetsBySize[2], expectedCandidateDissociativeItems[2]);
            CollectionAssert.AreEquivalent(candidateDissociativeSetsBySize[3], expectedCandidateDissociativeItems[3]);
        }

        [Test]
        public void GenerateDissocativeRulesTest()
        {
            // Given 
            var subject = new DissociationRulesFinder<string>();
            ITransactionsSet<string> abstractSet = AbstractTransactionsSet;
            var miningParams = new DisociativeRulesMiningParams(0.4, maxRelativeJoin: 0.1, minimalConfidence: 1.0);

            var expectedDissociativeRules = new List<IDissociativeRule<string>>
            {
                new DissociativeRule<string>(
                    new FrequentItemsSet<string>(new object[] { 2, 5 }, new[] { "C" }, 2, 0.4),
                    new FrequentItemsSet<string>(new object[] { 1, 3, 4 }, new[] { "D" }, 3, 0.6), 2, 0.4, 1, 0),
                new DissociativeRule<string>(
                    new FrequentItemsSet<string>(new object[] { 2, 5 }, new[] { "C" }, 2, 0.4),
                    new FrequentItemsSet<string>(new object[] { 3, 4 }, new[] { "E" }, 2, 0.4), 2, 0.4, 1, 0),
                new DissociativeRule<string>(
                    new FrequentItemsSet<string>(new object[] { 1, 3, 4 }, new[] { "D" }, 3, 0.6),
                    new FrequentItemsSet<string>(new object[] { 2, 5 }, new[] { "C" }, 2, 0.4), 2, 0.4, 1, 0),
                new DissociativeRule<string>(
                    new FrequentItemsSet<string>(new object[] { 3, 4 }, new[] { "E" }, 2, 0.4),
                    new FrequentItemsSet<string>(new object[] { 2, 5 }, new[] { "C" }, 2, 0.4), 2, 0.4, 1, 0)
                    
            };

            // When
            var actualDissociativeRules = subject
                .FindDissociativeRules(abstractSet, miningParams)
                .OrderBy(itm => itm.Antecedent.ItemsSet.First()[0]).ThenBy(itm => itm.Consequent.ItemsSet.First()[0]);

            // Then
           CollectionAssert.AreEquivalent(expectedDissociativeRules, actualDissociativeRules);
        }
    }
}