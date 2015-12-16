namespace BrainSharper.Implementations.Algorithms.AssociationAnalysis.Apriori
{
    using System;
    using System.Collections;
    using System.Collections.Concurrent;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;

    using Abstract.Algorithms.AssociationAnalysis;
    using Abstract.Algorithms.AssociationAnalysis.DataStructures;
    using DataStructures;

    public class AprioriAlgorithm<TValue> : IFrequentItemsFinder<TValue>
    {
        public IFrequentItemsSet<IFrequentItemsSet<TValue>> FindFrequentItems(ITransactionsSet<TValue> transactionsSet, IAssociationMiningParams associationMiningParams)
        {
            
            throw new NotImplementedException();
        }

        public IList<IFrequentItemsSet<TValue>> GenerteCandidateItems(
            ITransactionsSet<TValue> transactionsSet,
            IAssociationMiningParams associationMiningParams,
            IList<IFrequentItemsSet<TValue>> previousItems,
            int desiredSize)
        {
            int kMinusOne = desiredSize - 2;
            var elementsComparator = Comparer<TValue>.Default;
            return (
                    from itm1 in previousItems
                    from itm2 in previousItems
                    where !itm2.Equals(itm1) && elementsComparator.Compare(itm1.OrderedItems[kMinusOne], itm2.OrderedItems[kMinusOne]) < 0 && 
                            (desiredSize == 2 || itm1.KFirstElementsEqual(itm2, kMinusOne))
                    select new FrequentItemsSet<TValue>(new SortedSet<TValue>(itm1.OrderedItems.Union(itm2.OrderedItems))) 
                   as IFrequentItemsSet<TValue>)
                   .ToList();

        }

        public IList<IFrequentItemsSet<TValue>> GenerateInitialItemsSet(
            ITransactionsSet<TValue> transactionsSet,
            IAssociationMiningParams associationMiningParams)
        {
            var elementsWithSupport = new ConcurrentDictionary<TValue, IList<object>>();
            var totalElemsCount = (double)transactionsSet.TransactionsCount;
            Parallel.ForEach(
                transactionsSet.TransactionsList,
                transaction =>
                    {
                        var transactionKey = transaction.TransactionKey;
                        foreach (var item in transaction.TransactionItems)
                        {
                            elementsWithSupport.AddOrUpdate(
                                item,
                                addValueFactory: value => new List<object> { transactionKey },
                                updateValueFactory: (newElem, existingList) =>
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
                    new FrequentItemsSet<TValue>(kvp.Value.Count, kvp.Value.Count / totalElemsCount, kvp.Value, kvp.Key) as IFrequentItemsSet<TValue>)
                    .Where(itm => itm.Support >= associationMiningParams.MinimalSupport)
                    .ToList();
        }

        public IList<IFrequentItemsSet<TValue>> SelectOnlyFrequentItemsSets(
            IList<IFrequentItemsSet<TValue>> candidateItems,
            IList<IFrequentItemsSet<TValue>> previousFrequentItems,
            int desiredSize,
            IAssociationMiningParams associationMiningParams)
        {
            var itemsetsToKeep = new List<IFrequentItemsSet<TValue>>();
            foreach (var candidateItemsSet in candidateItems)
            {
                
            }
            throw new NotImplementedException();
        }

        public IList<IFrequentItemsSet<TValue>> CalulateValuesForNewFrequentItemsSets(
            IList<IFrequentItemsSet<TValue>> newFrequentItemsSets,
            ITransactionsSet<TValue> transactionsSet,
            IAssociationMiningParams associationMiningParams)
        {
            throw new NotImplementedException();
        }



    }
}