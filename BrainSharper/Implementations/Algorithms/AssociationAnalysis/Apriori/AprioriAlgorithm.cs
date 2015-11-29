namespace BrainSharper.Implementations.Algorithms.AssociationAnalysis.Apriori
{
    using System;
    using System.Collections.Concurrent;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;

    using BrainSharper.Abstract.Algorithms.AssociationAnalysis;
    using BrainSharper.Abstract.Algorithms.AssociationAnalysis.DataStructures;
    using BrainSharper.Abstract.Data;
    using BrainSharper.Implementations.Algorithms.AssociationAnalysis.DataStructures;

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
            throw new NotImplementedException();
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
                    .ToList();
        }

        public IList<IFrequentItemsSet<TValue>> SelectOnlyFrequentItemsSets(
            IList<IFrequentItemsSet<TValue>> candidateItems,
            IList<IFrequentItemsSet<TValue>> previousFrequentItems,
            int desiredSize,
            IAssociationMiningParams associationMiningParams)
        {
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
