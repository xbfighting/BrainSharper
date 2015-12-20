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
    public class AprioriAlgorithm<TValue> : IFrequentItemsFinder<TValue>
    {
        public IList<IFrequentItemsSet<TValue>> FindFrequentItems(
            ITransactionsSet<TValue> transactionsSet,
            IAssociationMiningParams associationMiningParams)
        {
            var initialFrequentItems = GenerateInitialItemsSet(transactionsSet, associationMiningParams);
            var frequentItemsBySize = new Dictionary<int, IList<IFrequentItemsSet<TValue>>>
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
                var nextItems = GenerateNextItems(transactionsSet, associationMiningParams, previousItems, itemsSetSize);
                if (nextItems.Any())
                {
                    anyItemsGenerated = true;
                    frequentItemsBySize[itemsSetSize] = nextItems;
                }

            }
            return frequentItemsBySize.Values.SelectMany(itm => itm).ToList();
        }

        public IList<IFrequentItemsSet<TValue>> GenerateNextItems(
            ITransactionsSet<TValue> transactionsSet,
            IAssociationMiningParams associationMiningParams,
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
                where relativeSupport >= associationMiningParams.MinimalRelativeSupport
                select newItem
                ).ToList();
        }

        public IList<IFrequentItemsSet<TValue>> GenerateInitialItemsSet(
            ITransactionsSet<TValue> transactionsSet,
            IAssociationMiningParams associationMiningParams)
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
                    .Where(itm => itm.Support >= associationMiningParams.MinimalRelativeSupport)
                    .ToList();
        }
    }
}