using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using BrainSharper.Abstract.Algorithms.AssociationAnalysis;
using BrainSharper.Abstract.Algorithms.AssociationAnalysis.DataStructures;
using BrainSharper.Implementations.Algorithms.AssociationAnalysis.DataStructures.Common;
using BrainSharper.Implementations.Algorithms.AssociationAnalysis.DataStructures.FPGrowth;

namespace BrainSharper.Implementations.Algorithms.AssociationAnalysis.FPGrowth
{
    public class FpGrowthBuilder<TValue> : IFrequentItemsFinder<TValue>
    {
        public IFrequentItemsSearchResult<TValue> FindFrequentItems(
            ITransactionsSet<TValue> transactions,
            IFrequentItemsMiningParams frequentItemsMiningParams)
        {
            var initialItems = GenerateInitialFrequentItems(transactions, frequentItemsMiningParams);
            var transactonsWithOnlyFrequentElements = RemapTrasactionsToContainOnlyFrequentItems(transactions, initialItems, frequentItemsMiningParams);
            var fpGrowthTree = BuildFpTree(transactonsWithOnlyFrequentElements);
            throw new NotImplementedException();
        }

        IDictionary<TValue, IFrequentItemsSet<TValue>> GenerateInitialFrequentItems(ITransactionsSet<TValue> transactions, IFrequentItemsMiningParams frequentItemsMiningParams)
        {
            var elementsWithSupport = new ConcurrentDictionary<TValue, IList<object>>();

            Parallel.ForEach(transactions.TransactionsList, tran =>
            {
                Func<TValue, IList<object>, IList<object>> updateFunc = (o, list) => new List<object>(list) { tran.TransactionKey };

                foreach (var item in tran.TransactionItems)
                {
                    elementsWithSupport.AddOrUpdate(item, newVal => new List<object> { tran.TransactionKey }, updateFunc);
                }
            });

            return elementsWithSupport
                .Where(kvp => (kvp.Value.Count / (double)transactions.TransactionsCount) >= frequentItemsMiningParams.MinimalRelativeSupport)
                .ToDictionary(
                    kvp => kvp.Key, 
                    kvp => new FrequentItemsSet<TValue>(kvp.Value, new [] { kvp.Key }, kvp.Value.Count, kvp.Value.Count/(double)transactions.TransactionsCount) as IFrequentItemsSet<TValue>);
        }

        protected virtual ITransactionsSet<TValue> RemapTrasactionsToContainOnlyFrequentItems(
            ITransactionsSet<TValue> transactions,
            IDictionary<TValue, IFrequentItemsSet<TValue>> frequentItems,
            IFrequentItemsMiningParams miningParams)
        {
            var remappedTransactions = new ITransaction<TValue>[transactions.TransactionsCount];
            Parallel.For(0, transactions.TransactionsCount, tranIdx =>
            {
                var transaction = transactions.TransactionsList.ElementAt(tranIdx);
                var frequentElementsOnly = transaction.TransactionItems
                    .Where(frequentItems.ContainsKey)
                    .OrderByDescending(itm => frequentItems[itm].RelativeSupport)
                    .ToList();
                remappedTransactions[tranIdx] = new Transaction<TValue>(transaction.TransactionKey, frequentElementsOnly);
            });
            return new TransactionsSet<TValue>(remappedTransactions);
        }

        protected virtual FpGrowthNode<TValue> BuildFpTree(ITransactionsSet<TValue> transactions)
        {
            var tree = new FpGrowthNode<TValue>();
            foreach (var transaction in transactions.TransactionsList)
            {
                var transactionItems = transaction.TransactionItems;
                AddToTree(tree, transactionItems.ToList());
            }
            return tree;
        }

        protected virtual void AddToTree(FpGrowthNode<TValue> treeNode, IList<TValue> items)
        {
            if (!items.Any())
            {
                return;
            }

            TValue head = items.First();
            FpGrowthNode<TValue> nodeToAddTo = treeNode;
            var tail = items.Skip(1).ToList();

            if (!treeNode.HasChildren)
            {
                nodeToAddTo = BuildNewChild(treeNode, head);
            }
            else
            {
                var anyChildMatched = false;
                foreach (var child in treeNode.Children)
                {
                    if (child.Value.Equals(head))
                    {
                        child.IncrementCountBy(1);
                        nodeToAddTo = child;
                        anyChildMatched = true;
                        break;
                    }
                }
                if (!anyChildMatched)
                {
                    nodeToAddTo = BuildNewChild(treeNode, head);
                }
            }
            AddToTree(nodeToAddTo, tail);
        }

        private static FpGrowthNode<TValue> BuildNewChild(
            FpGrowthNode<TValue> treeNode, 
            TValue head)
        {
            var newChild = new FpGrowthNode<TValue>(head, true, 1);
            treeNode.AddChild(newChild);
            return newChild;
        }
    }
}
