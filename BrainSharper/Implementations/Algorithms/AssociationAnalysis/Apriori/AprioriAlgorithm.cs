using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Emit;
using System.Threading.Tasks;
using BrainSharper.Abstract.Algorithms.AssociationAnalysis;
using BrainSharper.Abstract.Algorithms.AssociationAnalysis.DataStructures;
using BrainSharper.Implementations.Algorithms.AssociationAnalysis.DataStructures;
using BrainSharper.General.Utils;

namespace BrainSharper.Implementations.Algorithms.AssociationAnalysis.Apriori
{
    public class AprioriAlgorithm<TValue> : IFrequentItemsFinder<TValue>, IAssociationRulesFinder<TValue>
    {
        protected readonly AssocRuleMiningMinimumRequirementsChecker<TValue> AssocRuleMiningRequirementsChecker;

        public AprioriAlgorithm(AssocRuleMiningMinimumRequirementsChecker<TValue> assocRuleMiningRequirementsChecker)
        {
            AssocRuleMiningRequirementsChecker = assocRuleMiningRequirementsChecker;
        }

        public AprioriAlgorithm() : this(AssociationMiningParamsInterpreter.AreMinimalRequirementsMet)
        {

        }

        public virtual IFrequentItemsSearchResult<TValue> FindFrequentItems(
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
                anyItemsGenerated = false;
                itemsSetSize += 1;
                var itemsGroupedByCriteria = ProcessNextSetOfItems(frequentItemsBySize, itemsSetSize, transactionsSet, frequentItemsMiningParams);
                if (itemsGroupedByCriteria.ContainsKey(true))
                {
                    frequentItemsBySize[itemsSetSize] = itemsGroupedByCriteria[true].Select(itm => itm.FrequentItemsSet).ToList();
                    anyItemsGenerated = true;
                }
            }
            return new FrequentItemsSearchResult<TValue>(frequentItemsBySize);
        }

        protected virtual IDictionary<bool, List<FrequentItemBuildingResultDto<TValue>>> ProcessNextSetOfItems(
            IDictionary<int, IList<IFrequentItemsSet<TValue>>>  frequentItemsSoFar,
            int itemsSetSize,
            ITransactionsSet<TValue> transactionsSet,
            IFrequentItemsMiningParams frequentItemsMiningParams
            )
        {
            
            var previousItems = frequentItemsSoFar[itemsSetSize - 1];
            var nextItems = GenerateNextItems(transactionsSet, frequentItemsMiningParams, previousItems, itemsSetSize);
            return (
                from buildingResult in nextItems.AsParallel()
                group buildingResult by buildingResult.MeetsCriteria
                into grp
                select grp).ToDictionary(grp => grp.Key, grp => grp.ToList());
        }

        public IList<FrequentItemBuildingResultDto<TValue>> GenerateNextItems(
            ITransactionsSet<TValue> transactionsSet,
            IFrequentItemsMiningParams frequentItemsMiningParams,
            IList<IFrequentItemsSet<TValue>> previousItems,
            int desiredSize)
        {
            var kMinusOne = desiredSize - 2;
            var elementsComparator = Comparer<TValue>.Default;
            return (
                from itm1 in previousItems.AsParallel()
                from itm2 in previousItems
                where
                    elementsComparator.Compare(itm1.OrderedItems[kMinusOne], itm2.OrderedItems[kMinusOne]) < 0 &&
                    (desiredSize == 2 || itm1.KFirstElementsEqual(itm2, kMinusOne))

                // optimization: use TIDsets to calculate all at once, without a need to reiterate dataset later
                let candidateItemsTidSet = itm1.TransactionIds.Intersect(itm2.TransactionIds).ToList()
                let relativeSupport = candidateItemsTidSet.Count/ (double)transactionsSet.TransactionsCount
                let newItem  = 
                    new FrequentItemsSet<TValue>(
                        candidateItemsTidSet,
                        new SortedSet<TValue>(itm1.OrderedItems.Union(itm2.OrderedItems)),
                        candidateItemsTidSet.Count, 
                        relativeSupport
                        ) as IFrequentItemsSet<TValue>
                let itemMeetsCriteria = CandidateItemMeetsCriteria(newItem, frequentItemsMiningParams)
                select new FrequentItemBuildingResultDto<TValue>(newItem, itemMeetsCriteria)
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
                    .Where(itm => CandidateItemMeetsCriteria(itm, frequentItemsMiningParams))
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

            var associationRules = 
                (from itemsSize in frequentItemsSearchResult.FrequentItemsSizes
                where itemsSize > 1
                let items = frequentItemsSearchResult[itemsSize]
                let rulesFromItems =
                    items.AsParallel().SelectMany(
                        itemsSet =>
                            ProduceAssociationRules(itemsSet, frequentItemsSearchResult, associationMiningParams))
                        .Where(r => AssocRuleMeetsCriteria(r, associationMiningParams))
                select rulesFromItems).SelectMany(r => r.ToList()).ToList();
                        //var associationRules = new ConcurrentBag<IAssociationRule<TValue>>();
            /*
            allSizes.AsParallel().ForAll(itemsSetSize =>
            {
                var itemsOfGivenSize = frequentItemsSearchResult[itemsSetSize];
                foreach (var itemsSet in itemsOfGivenSize)
                {

                    associationRules.Add(
                        ProduceAssociationRules(itemsSet, frequentItemsSearchResult, associationMiningParams)
                        .Where(candidateRule => AssocRuleMeetsCriteria(candidateRule, associationMiningParams)).ToList()
                        );
                }
            });
            */
            /*
            foreach (var itemsSetSize in Enumerable.Range(2, maxFrequentItemsSize))
            {
                var itemsOfGivenSize = frequentItemsSearchResult[itemsSetSize];
                itemsOfGivenSize.AsParallel().ForAll(itemsSet =>
                {
                    var assocRules = ProduceAssociationRules(itemsSet, frequentItemsSearchResult,
                        associationMiningParams)
                        .Where(candidateRule => AssocRuleMeetsCriteria(candidateRule, associationMiningParams));

                    foreach (var assocRule in assocRules)
                    {
                        associationRules.Add(assocRule);
                    }
                });
            }
            */
            return associationRules.ToList();
        }

        protected virtual IEnumerable<IAssociationRule<TValue>> ProduceAssociationRules(
            IFrequentItemsSet<TValue> currentItemSet,
            IFrequentItemsSearchResult<TValue> frequentItemsSearchResult,
            IAssociationMiningParams assocMiningParams)
        {
            ;
            var combinations = ProduceConsequents(currentItemSet, assocMiningParams);

            foreach (var possibleConsequent in combinations)
            {
                var possibleConsequentList = possibleConsequent.ToList();
                if (possibleConsequentList.Count == currentItemSet.OrderedItems.Count)
                {
                    continue;
                }
                var consequentSize = possibleConsequentList.Count;
                var consequentFrequentItem = frequentItemsSearchResult[consequentSize].First(itm => itm.ItemsSet.SetEquals(possibleConsequentList));
                var antecedentItemsSet = currentItemSet.ItemsSet.Except(possibleConsequentList).ToList();

                var antecedentFrequentItemsSet = frequentItemsSearchResult[antecedentItemsSet.Count].First(itm => itm.ItemsSet.SetEquals(antecedentItemsSet));

                var confidence = currentItemSet.RelativeSupport / antecedentFrequentItemsSet.RelativeSupport;
                var newAssociationRule = ConstructAssocRule(
                    currentItemSet,
                    antecedentFrequentItemsSet, 
                    consequentFrequentItem, 
                    assocMiningParams,
                    confidence);
                yield return newAssociationRule;
            }
        }

        protected virtual IEnumerable<IEnumerable<TValue>> ProduceConsequents(
            IFrequentItemsSet<TValue> currentItemSet, 
            IAssociationMiningParams assocMiningParams)
        {
            return assocMiningParams.AllowMultiSelectorConsequent
                ? currentItemSet.ItemsSet.GenerateAllCombinations()
                : currentItemSet.ItemsSet.GenerateCombinationsOfSizeK(1);
        }

        protected virtual bool CandidateItemMeetsCriteria(IFrequentItemsSet<TValue> itm, IFrequentItemsMiningParams miningParams)
        {
            return itm.RelativeSupport >= miningParams.MinimalRelativeSupport;
        }

        protected virtual bool AssocRuleMeetsCriteria(IAssociationRule<TValue> assocRule,
            IAssociationMiningParams miningParams)
        {
            return AssocRuleMiningRequirementsChecker(assocRule, miningParams);
        }

        protected virtual AssociationRule<TValue> ConstructAssocRule(
            IFrequentItemsSet<TValue> currentItemSet,
            IFrequentItemsSet<TValue> antecedentFrequentItemsSet, 
            IFrequentItemsSet<TValue> consequentFrequentItem, 
            IAssociationMiningParams assocMiningParams,
            double confidence)
        {
            var newAssociationRule = new AssociationRule<TValue>(
                antecedentFrequentItemsSet,
                consequentFrequentItem,
                currentItemSet.Support,
                currentItemSet.RelativeSupport,
                confidence);
            return newAssociationRule;
        }
    }
}