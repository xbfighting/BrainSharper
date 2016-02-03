using System.Collections.Generic;
using System.Linq;
using BrainSharper.Abstract.Algorithms.AssociationAnalysis;
using BrainSharper.Abstract.Algorithms.AssociationAnalysis.DataStructures;
using BrainSharper.General.Utils;
using BrainSharper.Implementations.Algorithms.AssociationAnalysis.DataStructures;

namespace BrainSharper.Implementations.Algorithms.AssociationAnalysis.Apriori
{
    public class DissociationRulesFinder<TValue> : AprioriAlgorithm<TValue>
    {
        public DissociationRulesFinder(
            AssocRuleMiningMinimumRequirementsChecker<TValue> assocRuleMiningRequirementsChecker) 
            : base(assocRuleMiningRequirementsChecker)
        {
        }

        public DissociationRulesFinder()
            : base(AssociationMiningParamsInterpreter.AreMinimalRequirementsMet)
        {
        }

        public IFrequentItemsWithNegativeboundarySearchResult<TValue> FindDissociativeAssociationRules(ITransactionsSet<TValue> transactionsSet,
            IFrequentItemsMiningParams frequentItemsMiningParams)
        {
            return null;
        }

        public override IFrequentItemsSearchResult<TValue> FindFrequentItems(ITransactionsSet<TValue> transactionsSet,
            IFrequentItemsMiningParams frequentItemsMiningParams)
        {
            var initialFrequentItems = GenerateInitialItemsSet(transactionsSet, frequentItemsMiningParams);
            var frequentItemsBySize = new Dictionary<int, IList<IFrequentItemsSet<TValue>>>
            {
                [1] = initialFrequentItems
            };
            var negativeBoundaryItemsBySize = new Dictionary<int, IList<IFrequentItemsSet<TValue>>>();

            var anyItemsGenerated = true;
            var itemsSetSize = 1;
            while (anyItemsGenerated)
            {
                anyItemsGenerated = false;
                itemsSetSize += 1;
                var itemsGroupedByCriteria = ProcessNextSetOfItems(frequentItemsBySize, itemsSetSize, transactionsSet, frequentItemsMiningParams);
                if (itemsGroupedByCriteria.ContainsKey(true))
                {
                    frequentItemsBySize[itemsSetSize] = itemsGroupedByCriteria[true].Select(itm => itm.FrequentItemsSet).ToList();
                    anyItemsGenerated = true;
                }

                if (itemsGroupedByCriteria.ContainsKey(false))
                {
                    var itemsNotMeetingCriteria =
                        itemsGroupedByCriteria[false].Select(res => res.FrequentItemsSet).ToList();
                    var negativeBoundaryItems = BuildNegativeBoundary(frequentItemsBySize, itemsNotMeetingCriteria);
                    if (negativeBoundaryItems.Any())
                    {
                        negativeBoundaryItemsBySize[itemsSetSize] = negativeBoundaryItems;
                    }
                }

            }
            return new FrequentItemsWithNegativeBoundarySearchResult<TValue>(
                frequentItemsBySize,
                negativeBoundaryItemsBySize
                );
        }

        protected IList<IFrequentItemsSet<TValue>> BuildNegativeBoundary(
            IDictionary<int, IList<IFrequentItemsSet<TValue>>> frequentItemsSoFar,
            IList<IFrequentItemsSet<TValue>> itemsNotMeetingCriteria
            )
        {
            var negativeBoundaryItems = new List<IFrequentItemsSet<TValue>>();
            foreach (var itemNotMeetingCriteria in itemsNotMeetingCriteria)
            {
                var belongsToNegativeBoundary = true;
                var subsets = itemNotMeetingCriteria.ItemsSet.GenerateAllCombinations();
                foreach (var elements in subsets)
                {
                    var subset = new HashSet<TValue>(elements);
                    if (subset.Count == itemNotMeetingCriteria.ItemsSet.Count)
                    {
                        continue;
                    }
                    var subsetIsFrequent = CheckIfSubsetIsFrequent(subset, frequentItemsSoFar);
                    if (!subsetIsFrequent)
                    {
                        belongsToNegativeBoundary = false;
                        break;
                    }
                }
                if (belongsToNegativeBoundary)
                {
                    negativeBoundaryItems.Add(itemNotMeetingCriteria);
                }
            }
            return negativeBoundaryItems;
        }

        protected bool CheckIfSubsetIsFrequent(
            ISet<TValue> subset,
            IDictionary<int, IList<IFrequentItemsSet<TValue>>> frequentItemsBySize
            )
        {
            if (!frequentItemsBySize.ContainsKey(subset.Count))
            {
                return false;
            }
            var frequentItemsSameSize = frequentItemsBySize[subset.Count];
            var subsetIsFrequent = frequentItemsSameSize
                .AsParallel()
                .Any(freqItm => freqItm.ItemsSet.SetEquals(subset));
            return subsetIsFrequent;
        }
    }
}