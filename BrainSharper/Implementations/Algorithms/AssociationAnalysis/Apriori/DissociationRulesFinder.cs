using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using BrainSharper.Abstract.Algorithms.AssociationAnalysis;
using BrainSharper.Abstract.Algorithms.AssociationAnalysis.DataStructures;
using BrainSharper.General.Utils;
using BrainSharper.Implementations.Algorithms.AssociationAnalysis.DataStructures;
using BrainSharper.Implementations.Data;

namespace BrainSharper.Implementations.Algorithms.AssociationAnalysis.Apriori
{
    public class DissociationRulesFinder<TValue> : AprioriAlgorithm<TValue>
    {
        private static string InvalidMiningParametersTypePassedToDissociationRulesFinderError = "Invalid mining parameters type passed to DissociationRulesFinder!";

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

        public override IFrequentItemsSearchResult<TValue> FindFrequentItems(
            ITransactionsSet<TValue> transactionsSet,
            IFrequentItemsMiningParams frequentItemsMiningParams)
        {
            if (!(frequentItemsMiningParams is IDissociationRulesMiningParams))
            {
                throw new ArgumentException(InvalidMiningParametersTypePassedToDissociationRulesFinderError);
            }
            var dissocRulesMiningParams = frequentItemsMiningParams as IDissociationRulesMiningParams;
            var initialFrequentItems = GenerateInitialItemsSet(transactionsSet, frequentItemsMiningParams);
            var frequentItemsBySize = new Dictionary<int, IList<IFrequentItemsSet<TValue>>>
            {
                [1] = initialFrequentItems
            };

            var anyItemsGenerated = true;
            var itemsSetSize = 1;
            var candidateDissociationItems = new List<DissociativeRuleCandidateItemSet<TValue>>(); 
            var negativeBoundaryItems = new List<DissociativeRuleCandidateItemSet<TValue>>();
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
                    var dissocItemsAndCandidates = BuildTwoItemsets(frequentItemsBySize, itemsNotMeetingCriteria, dissocRulesMiningParams);
                    negativeBoundaryItems.AddRange(dissocItemsAndCandidates.Item1);
                    candidateDissociationItems.AddRange(dissocItemsAndCandidates.Item2);
                }
            }
            IList<DissociativeRuleCandidateItemSet<TValue>> extendedCandidateDissociativeItems = ExtendCandidateDissociativeItems(
                frequentItemsBySize, 
                negativeBoundaryItems,
                candidateDissociationItems,
                transactionsSet);
            var extendedValidDissociativeItemsets = ExtendValidDissociativeItems(
                extendedCandidateDissociativeItems,
                negativeBoundaryItems, 
                frequentItemsBySize, 
                transactionsSet, 
                dissocRulesMiningParams);
            var dissociativeRules = BuildDissociativeRules(extendedValidDissociativeItemsets, dissocRulesMiningParams);
            Debug.Assert(dissociativeRules.Count > 0);
            //TODO: solve problem with return type and wrapping it all at once
            return null;
        }

        protected IList<DissociativeRuleCandidateItemSet<TValue>> ExtendCandidateDissociativeItems(
            Dictionary<int, IList<IFrequentItemsSet<TValue>>> frequentItemsBySize,
            IList<DissociativeRuleCandidateItemSet<TValue>> dissociativeItemsets,
            IList<DissociativeRuleCandidateItemSet<TValue>> candidateDissociativeItemsets,
            ITransactionsSet<TValue> transactionsSet)
        {
            var anyNewItemsFound = true;
            var newItemSets = new List<DissociativeRuleCandidateItemSet<TValue>>();
            var latestItems = candidateDissociativeItemsets;
            while (anyNewItemsFound)
            {
                var newItems = new List<DissociativeRuleCandidateItemSet<TValue>>();
                foreach (var candidateItemsSet in latestItems)
                {
                    foreach (var frequentItemSizeOne in frequentItemsBySize[1])
                    {
                        //TODO: wrap it nicely
                        if (!candidateItemsSet.FirstSubset.ItemsSet.Contains(frequentItemSizeOne.ItemsSet.First()) &&
                            !candidateItemsSet.SecondSubset.ItemsSet.Contains(frequentItemSizeOne.ItemsSet.First()))
                        {
                            var extendedFirstSet = new HashSet<TValue>(candidateItemsSet.FirstSubset.ItemsSet);
                            extendedFirstSet.Add(frequentItemSizeOne.ItemsSet.First());
                            var isFirstExtendedSetFrequent = CheckIfSetIsFrequent(extendedFirstSet, frequentItemsBySize);
                            if (isFirstExtendedSetFrequent.Item1)
                            {
                                var extendedSecondSet = new HashSet<TValue>(candidateItemsSet.SecondSubset.ItemsSet);
                                extendedSecondSet.Add(frequentItemSizeOne.ItemsSet.First());
                                var isSecondExtendedSetFrequent = CheckIfSetIsFrequent(extendedSecondSet, frequentItemsBySize);
                                if (isSecondExtendedSetFrequent.Item1)
                                {
                                    var extendedFullSet =
                                        candidateItemsSet.AllItemsSet.Union(frequentItemSizeOne.ItemsSet);
                                    var isFullSetFrequent = CheckIfSetIsFrequent(extendedFullSet, frequentItemsBySize);
                                    if (!isFullSetFrequent.Item1)
                                    {
                                        newItems.Add(new DissociativeRuleCandidateItemSet<TValue>(
                                            candidateItemsSet.FirstSubset,
                                            isSecondExtendedSetFrequent.Item2
                                        ));
                                        newItems.Add(new DissociativeRuleCandidateItemSet<TValue>(
                                            isFirstExtendedSetFrequent.Item2,
                                            candidateItemsSet.SecondSubset
                                            ));
                                    }
                                }
                            }
                        }
                    }
                }
                anyNewItemsFound = newItems.Any();
                CalculateSupportOfDissociationRuleCandidateItemSets(transactionsSet, newItemSets);
                newItemSets.AddRange(newItems);
                latestItems = newItems;
            }

            return dissociativeItemsets.Union(newItemSets).ToList();
        }

        protected IList<DissociativeRuleCandidateItemSet<TValue>> ExtendValidDissociativeItems(
            IList<DissociativeRuleCandidateItemSet<TValue>> extendedCandidateItemSets,
            IList<DissociativeRuleCandidateItemSet<TValue>> validDissociativeItems,
            Dictionary<int, IList<IFrequentItemsSet<TValue>>> frequentItemsBySize,
            ITransactionsSet<TValue> transactionsSet,
            IDissociationRulesMiningParams miningParams)
        {
            var allValidDissociativeItems =
                validDissociativeItems.Union(
                    extendedCandidateItemSets.Where(set => set.RelativeSupport <= miningParams.MaxRelativeJoin)).ToList();
            var supersets = new List<DissociativeRuleCandidateItemSet<TValue>>();
            foreach (var validDissociationItemsSet in allValidDissociativeItems)
            {
                var itemSetSize = validDissociationItemsSet.AllItemsSet.Count;
                var maxFrequentItemsSize = frequentItemsBySize.Keys.Max();
                var supersetsOfThisSet = new List<DissociativeRuleCandidateItemSet<TValue>>();
                if (maxFrequentItemsSize > itemSetSize)
                {
                    for (int size = itemSetSize + 1; size < maxFrequentItemsSize; size++)
                    {
                        var frequentItemsOfGivenSize = frequentItemsBySize[size];
                        var supersetsOfSet =
                            frequentItemsOfGivenSize.Where(
                                itm => itm.ItemsSet.IsSupersetOf(validDissociationItemsSet.AllItemsSet));
                        foreach (var superset in supersetsOfSet)
                        {
                            var firstSubsetPart =
                                superset.ItemsSet.Except(validDissociationItemsSet.SecondSubset.ItemsSet);
                            var isFirstSubsetFrequent = CheckIfSetIsFrequent(firstSubsetPart, frequentItemsBySize);

                            var secondSubsetPart = superset.ItemsSet.Except(validDissociationItemsSet.FirstSubset.ItemsSet);
                            var isSecondSubsetFrequent = CheckIfSetIsFrequent(secondSubsetPart, frequentItemsBySize);

                            //TODO: check this assumptions with theoretical algorithm description
                            if (isFirstSubsetFrequent.Item1 && isSecondSubsetFrequent.Item1)
                            {
                                supersetsOfThisSet.Add(new DissociativeRuleCandidateItemSet<TValue>(isFirstSubsetFrequent.Item2, isSecondSubsetFrequent.Item2));
                            }
                        }
                    }
                }
                CalculateSupportOfDissociationRuleCandidateItemSets(transactionsSet, supersetsOfThisSet);
                supersets.AddRange(supersetsOfThisSet.Where(superset => superset.RelativeSupport <= miningParams.MaxRelativeJoin));
            }
            return validDissociativeItems.Union(supersets).ToList();

        }

        protected IList<IDissociativeRule<TValue>> BuildDissociativeRules(
            IList<DissociativeRuleCandidateItemSet<TValue>> dissociativeItemsets,
            IDissociationRulesMiningParams miningParams)
        {
            var dissociationRules = new List<IDissociativeRule<TValue>>();
            foreach (var dissociativeItemsSet in dissociativeItemsets)
            {
                var ruleRelativeSupport =
                    Enumerable.Min(new[]
                    {
                        dissociativeItemsSet.FirstSubset.RelativeSupport, dissociativeItemsSet.SecondSubset.RelativeSupport
                    });
                var ruleSupport =
                    Enumerable.Min(new[]
                    {
                        dissociativeItemsSet.FirstSubset.Support, dissociativeItemsSet.SecondSubset.Support
                    });

                var dissociativeImplications = CalculateDissociativeConfidence(dissociativeItemsSet);
                if (dissociativeImplications.Item1 >= miningParams.MinimalConfidence)
                {
                    dissociationRules.Add(BuildValidDissociativeRule(dissociativeItemsSet.FirstSubset, dissociativeItemsSet.SecondSubset, ruleSupport, ruleRelativeSupport, dissociativeImplications.Item1, dissociativeItemsSet.RelativeSupport.Value));
                }
                if (dissociativeImplications.Item2 >= miningParams.MinimalConfidence)
                {
                    dissociationRules.Add(BuildValidDissociativeRule(dissociativeItemsSet.SecondSubset, dissociativeItemsSet.FirstSubset, ruleSupport, ruleRelativeSupport, dissociativeImplications.Item2, dissociativeItemsSet.RelativeSupport.Value));
                }
            }
            return dissociationRules;
        }

        protected IDissociativeRule<TValue> BuildValidDissociativeRule(
            IFrequentItemsSet<TValue> antecedent,
            IFrequentItemsSet<TValue> consequent, 
            double support, 
            double relativeSupport, 
            double confidence,
            double maxRelativeJoin)
        {
            return new DissociativeRule<TValue>(
                antecedent,
                consequent,
                support,
                relativeSupport,
                confidence,
                maxRelativeJoin);
        }

        protected Tuple<IList<DissociativeRuleCandidateItemSet<TValue>>, IList<DissociativeRuleCandidateItemSet<TValue>>> BuildTwoItemsets(
            IDictionary<int, IList<IFrequentItemsSet<TValue>>> frequentItemsSoFar,
            IList<IFrequentItemsSet<TValue>> itemsNotMeetingCriteria,
            IDissociationRulesMiningParams miningParams)
        {
            var correctDissociationItemsets = new List<DissociativeRuleCandidateItemSet<TValue>>();
            var candidateDissociationItemsets = new List<DissociativeRuleCandidateItemSet<TValue>>();
            foreach (var itemNotMeetingCriteria in itemsNotMeetingCriteria)
            {
                var itemHalfCount = itemNotMeetingCriteria.ItemsSet.Count/2;
                var allSubsetsAreFrequent = true;
                var subsets = itemNotMeetingCriteria.ItemsSet.GenerateAllCombinations().AsParallel().OrderBy(itm => itm.Count()).ToList();
                var checkedItemsetsPairs = new List<DissociativeRuleCandidateItemSet<TValue>>();

                foreach (var subsetElements in subsets)
                {
                    if (subsetElements.Count() > itemHalfCount)
                    {
                        break;
                    }
                    var remainingElements = new HashSet<TValue>(itemNotMeetingCriteria.ItemsSet.Except(subsetElements));
                    if (subsetElements.Count() == itemNotMeetingCriteria.ItemsSet.Count)
                    {
                        continue;
                    }
                    var subsetFrequentEquivalent = CheckIfSetIsFrequent(subsetElements, frequentItemsSoFar);
                    var remainingElementsFrequentEquivalent = CheckIfSetIsFrequent(remainingElements, frequentItemsSoFar);
                    if (!subsetFrequentEquivalent.Item1 || !remainingElementsFrequentEquivalent.Item1)
                    {
                        allSubsetsAreFrequent = false;
                        break;
                    }
                    checkedItemsetsPairs.Add(
                        new DissociativeRuleCandidateItemSet<TValue>(
                            subsetFrequentEquivalent.Item2, 
                            remainingElementsFrequentEquivalent.Item2, 
                            itemNotMeetingCriteria.Support,
                            itemNotMeetingCriteria.RelativeSupport));
                    if (itemNotMeetingCriteria.ItemsSet.Count == 2)
                    {
                        break;
                    }
                }
                if (allSubsetsAreFrequent)
                {
                    var collectionToAdd = itemNotMeetingCriteria.Support < miningParams.MaxRelativeJoin
                        ? correctDissociationItemsets
                        : candidateDissociationItemsets;
                    collectionToAdd.AddRange(checkedItemsetsPairs);
                }
            }
            return
                new Tuple<IList<DissociativeRuleCandidateItemSet<TValue>>, IList<DissociativeRuleCandidateItemSet<TValue>>>(
                    correctDissociationItemsets, 
                    candidateDissociationItemsets);
        }

        protected Tuple<bool, IFrequentItemsSet<TValue>>  CheckIfSetIsFrequent(
            IEnumerable<TValue> subset,
            IDictionary<int, IList<IFrequentItemsSet<TValue>>> frequentItemsBySize
            )
        {
            if (!frequentItemsBySize.ContainsKey(subset.Count()))
            {
                return new Tuple<bool, IFrequentItemsSet<TValue>>(false, null);
            }
            var frequentItemsSameSize = frequentItemsBySize[subset.Count()];
            var frequentSubset = frequentItemsSameSize
                .AsParallel()
                .FirstOrDefault(freqItm => freqItm.ItemsSet.SetEquals(subset));
            return new Tuple<bool, IFrequentItemsSet<TValue>>(frequentSubset != null, frequentSubset);
        }

        protected static Tuple<double, double> CalculateDissociativeConfidence(DissociativeRuleCandidateItemSet<TValue> itemsSet)
        {
            if (itemsSet.RelativeSupport.HasValue)
            {
                var firstNotImpliesSecondConfidence = 1.0 -
                                                      (itemsSet.RelativeSupport.Value/
                                                       itemsSet.FirstSubset.RelativeSupport);
                var secondNotImpliesFirstConfidence = 1.0 -
                                                      (itemsSet.RelativeSupport.Value/
                                                       itemsSet.SecondSubset.RelativeSupport);
                return new Tuple<double, double>(firstNotImpliesSecondConfidence, secondNotImpliesFirstConfidence);
            }
            else
            {
                return new Tuple<double, double>(0.0, 0.0);
            }
            
        }

        protected void CalculateSupportOfDissociationRuleCandidateItemSets(
            ITransactionsSet<TValue> transactionsSet,
            IList<DissociativeRuleCandidateItemSet<TValue>> candidateItemSets)
        {
            Parallel.ForEach(candidateItemSets, itm =>
            {
                var supportCounter = 0.0;
                foreach (var transaction in transactionsSet.TransactionsList)
                {
                    if (itm.AllItemsSet.IsSupersetOf(transaction.TransactionItems))
                    {
                        supportCounter += 1;
                    }
                }
                itm.Support = supportCounter;
                itm.RelativeSupport = supportCounter/transactionsSet.TransactionsCount;
            });
        }
    }
}