using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using BrainSharper.Abstract.Algorithms.AssociationAnalysis;
using BrainSharper.Abstract.Algorithms.AssociationAnalysis.DataStructures;
using BrainSharper.Implementations.Algorithms.AssociationAnalysis.DataStructures;

namespace BrainSharper.Implementations.Algorithms.AssociationAnalysis.Apriori
{
    using BrainSharper.General.Utils;

    public class AprioriAlgorithm<TValue> : IFrequentItemsFinder<TValue>, IAssociationRulesFinder<TValue>
    {
        private MiningMinimumRequirementsChecker<TValue> _minimumMiningRequirementsChecker;

        public AprioriAlgorithm(MiningMinimumRequirementsChecker<TValue> minimumMiningRequirementsChecker)
        {
            this._minimumMiningRequirementsChecker = minimumMiningRequirementsChecker;
        }

        public AprioriAlgorithm()
            : this(AssociationMiningParamsInterpreter.AreMinimalRequirementsMet)
        {
        }

        public IFrequentItemsSearchResult<TValue> FindFrequentItems(
            ITransactionsSet<TValue> transactionsSet,
            IFrequentItemsMiningParams frequentItemsMiningParams)
        {
            var initialFrequentItems = GenerateInitialItemsSet(transactionsSet, frequentItemsMiningParams);
            var frequentItemsBySize = new Dictionary <int, IList<IFrequentItemsSet<TValue>>>
            {
                [1] = initialFrequentItems
            };
            var anyItemsGenerated = true;
            var itemsSetSize = 1;
            while (anyItemsGenerated)
            {
                itemsSetSize += 1;
                anyItemsGenerated = false;
                var previousItems = frequentItemsBySize[itemsSetSize - 1];
                var nextItems = GenerateNextItems(transactionsSet, frequentItemsMiningParams, previousItems, itemsSetSize);
                if (nextItems.Any())
                {
                    anyItemsGenerated = true;
                    frequentItemsBySize[itemsSetSize] = nextItems;
                }

            }
            return new FrequentItemsSearchResult<TValue>(frequentItemsBySize);
        }

        public IList<IFrequentItemsSet<TValue>> GenerateNextItems(
            ITransactionsSet<TValue> transactionsSet,
            IFrequentItemsMiningParams frequentItemsMiningParams,
            IList<IFrequentItemsSet<TValue>> previousItems,
            int desiredSize)
        {
            var kMinusOne = desiredSize - 2;
            var elementsComparator = Comparer<TValue>.Default;
            return (
                from itm1 in previousItems
                from itm2 in previousItems
                where
                    !itm2.Equals(itm1) &&
                    elementsComparator.Compare(itm1.OrderedItems[kMinusOne], itm2.OrderedItems[kMinusOne]) < 0 &&
                    (desiredSize == 2 || itm1.KFirstElementsEqual(itm2, kMinusOne))

                // optimization: use TIDsets to calculate all at once, without a need to reiterate dataset later
                let candidateItemsTidSet = itm1.TransactionIds.Intersect(itm2.TransactionIds).ToList()
                let relativeSupport = candidateItemsTidSet.Count/ (double)transactionsSet.TransactionsCount
                let newItem  =
                    new FrequentItemsSet<TValue>(
                        candidateItemsTidSet.Count, 
                        relativeSupport, 
                        candidateItemsTidSet, 
                        new SortedSet<TValue>(itm1.OrderedItems.Union(itm2.OrderedItems))) as IFrequentItemsSet<TValue>
                where relativeSupport >= frequentItemsMiningParams.MinimalRelativeSupport
                select newItem
                ).ToList();
        }

        public IList<IFrequentItemsSet<TValue>> GenerateInitialItemsSet(
            ITransactionsSet<TValue> transactionsSet,
            IFrequentItemsMiningParams frequentItemsMiningParams)
        {
            var elementsWithSupport = new ConcurrentDictionary<TValue, IList<object>>();
            var totalElemsCount = (double) transactionsSet.TransactionsCount;
            Parallel.ForEach(
                transactionsSet.TransactionsList,
                transaction =>
                {
                    var transactionKey = transaction.TransactionKey;
                    foreach (var item in transaction.TransactionItems)
                    {
                        elementsWithSupport.AddOrUpdate(
                            item, value => new List<object> {transactionKey}, (newElem, existingList) =>
                            {
                                var newList = existingList.ToList();
                                newList.Add(transactionKey);
                                return newList;
                            });
                    }
                });
            return
                elementsWithSupport.Select(
                    kvp =>
                        new FrequentItemsSet<TValue>(kvp.Value.Count, kvp.Value.Count/totalElemsCount, kvp.Value,
                            kvp.Key) as IFrequentItemsSet<TValue>)
                    .Where(itm => itm.Support >= frequentItemsMiningParams.MinimalRelativeSupport)
                    .ToList();
        }

        public IList<IAssociationRule<TValue>> FindAssociationRules(
            ITransactionsSet<TValue> transactionSet, 
            IFrequentItemsSearchResult<TValue> frequentItemsSearchResult,
            IAssociationMiningParams associationMiningParams)
        {
            var maxFrequentItemsSize = frequentItemsSearchResult.FrequentItemsSizes.Max();
            if (maxFrequentItemsSize == 1)
            {
                return new List<IAssociationRule<TValue>>();
            }

            var associationRules = new List<IAssociationRule<TValue>>();
            foreach(var itemsSetSize in Enumerable.Range(2, maxFrequentItemsSize))
            {
                var itemsOfGivenSize = frequentItemsSearchResult[itemsSetSize];
                foreach (var itemsSet in itemsOfGivenSize)
                {
                    associationRules
                        .AddRange(ProduceAssociationRules(itemsSet, frequentItemsSearchResult, associationMiningParams)
                        .Where(candidateRule => _minimumMiningRequirementsChecker(candidateRule, associationMiningParams)));
                }
            }
            return associationRules;
        }

        protected IEnumerable<IAssociationRule<TValue>> ProduceAssociationRules(
            IFrequentItemsSet<TValue> currentItemSet,
            IFrequentItemsSearchResult<TValue> frequentItemsSearchResult,
            IAssociationMiningParams frequentItemsMiningParams
            )
        {
            ;
            var combinations = frequentItemsMiningParams.AllowMultiSelectorConsequent
                                   ? currentItemSet.ItemsSet.GenerateAllCombinations()
                                   : currentItemSet.ItemsSet.GenerateCombinationsOfSizeK(1);

            foreach (var possibleConsequent in combinations)
            {
                if (possibleConsequent.Count() == currentItemSet.OrderedItems.Count)
                {
                    continue;
                }
                var consequentSize = possibleConsequent.Count();
                var consequentFrequentItem = frequentItemsSearchResult[consequentSize].First(itm => itm.ItemsSet.SetEquals(possibleConsequent));
                var antecedentItemsSet = currentItemSet.ItemsSet.Except(possibleConsequent).ToList();

                var antecedentFrequentItemsSet = frequentItemsSearchResult[antecedentItemsSet.Count].First(itm => itm.ItemsSet.SetEquals(antecedentItemsSet));

                var confidence = currentItemSet.RelativeSuppot / antecedentFrequentItemsSet.RelativeSuppot;
                var newAssociationRule = new AssociationRule<TValue>(
                    antecedentFrequentItemsSet,
                    consequentFrequentItem,
                    currentItemSet.Support,
                    currentItemSet.RelativeSuppot,
                    confidence);
                yield return newAssociationRule;
            }
        }
    }
}