using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using BrainSharper.Abstract.Algorithms.AssociationAnalysis;
using BrainSharper.Abstract.Algorithms.AssociationAnalysis.DataStructures;
using BrainSharper.General.Utils;
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
            var fpGrowthModel = BuildFpModel(transactonsWithOnlyFrequentElements);
            var frequentItems = PerformFrequentItemsMining(initialItems.Values.ToList(), fpGrowthModel, frequentItemsMiningParams, transactions.TransactionsCount);
            var itemsGroupedBySize = frequentItems
                .AsParallel()
                .GroupBy(itm => itm.ItemsSet.Count, itm => itm)
                .ToDictionary(kvp => kvp.Key, kvp => kvp.ToList() as IList<IFrequentItemsSet<TValue>>);
            return new FrequentItemsSearchResult<TValue>(itemsGroupedBySize);
        }

        public IDictionary<TValue, IFrequentItemsSet<TValue>> GenerateInitialFrequentItems(ITransactionsSet<TValue> transactions, IFrequentItemsMiningParams frequentItemsMiningParams)
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
                    kvp => new FrequentItemsSet<TValue>(kvp.Value, new[] { kvp.Key }, kvp.Value.Count, kvp.Value.Count / (double)transactions.TransactionsCount) as IFrequentItemsSet<TValue>);
        }

        public virtual ITransactionsSet<TValue> RemapTrasactionsToContainOnlyFrequentItems(
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
                    .OrderByDescending(itm => frequentItems[itm].Support)
                    .ThenByDescending(itm => itm)
                    .ToList();
                remappedTransactions[tranIdx] = new Transaction<TValue>(transaction.TransactionKey, frequentElementsOnly);
            });
            var noElemes = remappedTransactions.Where(tran => tran.TransactionItems.Any());
            Debug.Assert(noElemes.Any());
            return new TransactionsSet<TValue>(remappedTransactions.Where(tran => tran.TransactionItems.Any()));
        }

        public virtual FpGrowthModel<TValue> BuildFpModel(ITransactionsSet<TValue> transactions)
        {
            var headersDictionary = new Dictionary<TValue, IList<FpGrowthNode<TValue>>>();
            var tree = new FpGrowthNode<TValue>();
            foreach (var transaction in transactions.TransactionsList)
            {
                var transactionItems = transaction.TransactionItems;
                AddToTree(tree, transactionItems.Select(itm => new Tuple<TValue, double>(itm, 1)).ToList(), headersDictionary);
            }
            return new FpGrowthModel<TValue>(new FpGrowthHeaderTable<TValue>(headersDictionary), tree);
        }

        public virtual void AddToTree(
            FpGrowthNode<TValue> treeNode,
            IList<Tuple<TValue, double>> itemsWithCounts,
            IDictionary<TValue, IList<FpGrowthNode<TValue>>> headers)
        {
            if (!itemsWithCounts.Any())
            {
                return;
            }

            Tuple<TValue, double> headWithCount = itemsWithCounts.First();
            FpGrowthNode<TValue> nodeToAddTo = treeNode;
            var tail = itemsWithCounts.Skip(1).ToList();

            if (!treeNode.HasChildren)
            {
                nodeToAddTo = BuildNewChild(treeNode, headWithCount);
                AddNodeToHeadersTable(nodeToAddTo, headers);
            }
            else
            {
                var anyChildMatched = false;
                foreach (var child in treeNode.Children)
                {
                    if (child.Value.Equals(headWithCount.Item1))
                    {
                        child.IncrementCountBy(headWithCount.Item2);
                        nodeToAddTo = child;
                        anyChildMatched = true;
                        break;
                    }
                }
                if (!anyChildMatched)
                {
                    nodeToAddTo = BuildNewChild(treeNode, headWithCount);
                    AddNodeToHeadersTable(nodeToAddTo, headers);
                }
            }
            AddToTree(nodeToAddTo, tail, headers);
        }

        public virtual IEnumerable<IFrequentItemsSet<TValue>> PerformFrequentItemsMining(
            IList<IFrequentItemsSet<TValue>> initialItemsSet,
            FpGrowthModel<TValue> model,
            IFrequentItemsMiningParams miningParams,
            int totalItemsCount)
        {
            var allItems = new List<IFrequentItemsSet<TValue>>();
            foreach (var initialItem in initialItemsSet)
            {
                var itemValue = initialItem.OrderedItems.First();
                //  allItems.AddRange(ProcessPrefixTrees(itemValue, model, initialItem, miningParams, totalItemsCount));
                //  allItems.AddRange(ProcessPrefixPaths(itemValue, model, initialItem, miningParams, totalItemsCount));
                allItems.AddRange(ProcessPrefixPathForValue(itemValue, model, initialItem, miningParams, totalItemsCount));
            }
            return allItems;
        }

        public virtual IEnumerable<IFrequentItemsSet<TValue>> ProcessPrefixPathForValue(
            TValue valueToStartFrom,
            FpGrowthModel<TValue> currentModel,
            IFrequentItemsSet<TValue> currentPrefix,
            IFrequentItemsMiningParams miningParams,
            int totalTransactionsCount)
        {
            var transformedPaths = new List<IList<Tuple<TValue, double>>>();
            var valuesCountsMnemonics = new Dictionary<TValue, double>();
            double minimalTransactionsCount = totalTransactionsCount * miningParams.MinimalRelativeSupport;
            Func<double, double, double> updateF = (old, newV) => old + newV;
            foreach (var node in currentModel.HeaderTable.GetNodesForValue(valueToStartFrom))
            {
                var prefixPath = node.GetPathToSelf().ToList();
                valuesCountsMnemonics.AddOrUpdate(node.Value, node.Count, updateF);
                var nodeNodeNormalizedPrefixPath = new List<Tuple<TValue, double>>();
                foreach (var nodeAntecedent in prefixPath)
                {
                    var countToRecord = nodeAntecedent.Count > node.Count ? node.Count : nodeAntecedent.Count;
                    nodeNodeNormalizedPrefixPath.Add(new Tuple<TValue, double>(nodeAntecedent.Value, countToRecord));
                    valuesCountsMnemonics.AddOrUpdate(nodeAntecedent.Value, countToRecord, updateF);
                }
                if (nodeNodeNormalizedPrefixPath.Any()) transformedPaths.Add(nodeNodeNormalizedPrefixPath);
            }

            var extendedPrefix = new FrequentItemsSet<TValue>(
                currentPrefix.ItemsSet.Union(new List<TValue> { valueToStartFrom }),
                valuesCountsMnemonics[valueToStartFrom],
                valuesCountsMnemonics[valueToStartFrom] / totalTransactionsCount);
            yield return extendedPrefix;

            if (transformedPaths.Any())
            {
                var onlyFrequentPathElements = transformedPaths
                .Select(path =>
                            path
                                .Where(itm => valuesCountsMnemonics[itm.Item1] >= minimalTransactionsCount)
                                .OrderByDescending(itm => itm.Item2)
                                .ThenByDescending(itm => valuesCountsMnemonics[itm.Item1])
                                .ToList()).ToList();
                var headerTableSeed = new FpGrowthHeaderTable<TValue>();
                var prefixTree = onlyFrequentPathElements.Aggregate(new FpGrowthNode<TValue>(),
                    (tree, transaction) =>
                    {
                        AddToTree(tree, transaction, headerTableSeed.ValuesToNodesMapping);
                        return tree;
                    });
                var prefixModel = new FpGrowthModel<TValue>(headerTableSeed, prefixTree);
                foreach (var prefixValue in prefixModel.HeaderTable.ValuesToNodesMapping.Keys)
                {
                    var subPrefixes = ProcessPrefixPathForValue(prefixValue, prefixModel, extendedPrefix, miningParams,
                        totalTransactionsCount);
                    foreach (var subPrefix in subPrefixes) yield return subPrefix;
                }
            }
        }

        public virtual IEnumerable<IFrequentItemsSet<TValue>> ProcessPrefixPaths(
            TValue valueToStartFrom,
            FpGrowthModel<TValue> currentModel,
            IFrequentItemsSet<TValue> prefix,
            IFrequentItemsMiningParams miningParams,
            int totalTransactionsCount)
        {
            var transformedPaths = new List<IList<Tuple<TValue, double>>>();
            var valuesCountsMnemonics = new Dictionary<TValue, double>();
            var minimalTransactionsCount = (int)totalTransactionsCount * miningParams.MinimalRelativeSupport;
            foreach (var nodeWithStartValue in currentModel.HeaderTable.GetNodesForValue(valueToStartFrom))
            {
                var maxSupportValue = nodeWithStartValue.Count;
                var prefixPath = new List<Tuple<TValue, double>>();
                if (nodeWithStartValue.HasParent && !nodeWithStartValue.IsRoot)
                {
                    var processedNode = nodeWithStartValue;
                    while (processedNode.HasParent)
                    {
                        var parentNode = processedNode.Parent;
                        if (parentNode.IsRoot || prefix.ItemsSet.Contains(parentNode.Value))
                        {
                            break;
                        }
                        var countValue = parentNode.Count > maxSupportValue ? maxSupportValue : parentNode.Count;
                        prefixPath.Add(new Tuple<TValue, double>(parentNode.Value, countValue));
                        if (!valuesCountsMnemonics.ContainsKey(parentNode.Value))
                            valuesCountsMnemonics.Add(parentNode.Value, 0.0);
                        valuesCountsMnemonics[parentNode.Value] += countValue;
                        processedNode = parentNode;
                    }
                }
                prefixPath.Reverse();
                transformedPaths.Add(prefixPath);
            }
            var onlyFrequentItems = transformedPaths
                .Select(path => path
                    .Where(elem => valuesCountsMnemonics[elem.Item1] >= minimalTransactionsCount)
                    .OrderByDescending(elem => valuesCountsMnemonics[elem.Item1])
                    .ThenByDescending(elem => elem.Item1)
                    .ToList());
            var headerTableSeed = new FpGrowthHeaderTable<TValue>();
            var resultingTree = onlyFrequentItems.Aggregate(new FpGrowthNode<TValue>(),
                (tree, transaction) =>
                {
                    AddToTree(tree, transaction, headerTableSeed.ValuesToNodesMapping);
                    return tree;
                });
            var subModel = new FpGrowthModel<TValue>(headerTableSeed, resultingTree);
            if (resultingTree.HasChildren)
            {
                var nodesToProcessQueue = new Queue<FpGrowthNode<TValue>>(new[] { resultingTree });
                while (nodesToProcessQueue.Any())
                {
                    var head = nodesToProcessQueue.Dequeue();
                    nodesToProcessQueue = new Queue<FpGrowthNode<TValue>>(nodesToProcessQueue.Union(head.Children));
                    if (head.IsRoot) continue;
                    var pattern = prefix.ItemsSet.Union(new[] { head.Value });
                    var support = head.Count;
                    var relativeSupport = support / (double)totalTransactionsCount;
                    var conditionalPatternSet = new FrequentItemsSet<TValue>(pattern, support, relativeSupport);
                    yield return conditionalPatternSet;
                    foreach (
                        var subPattern in
                            ProcessPrefixPaths(head.Value, subModel, conditionalPatternSet, miningParams,
                                totalTransactionsCount)) yield return subPattern;
                }
            }
        }

        public virtual IEnumerable<IFrequentItemsSet<TValue>> ProcessPrefixTrees(
            TValue valueToStartFrom,
            FpGrowthModel<TValue> currentModel,
            IFrequentItemsSet<TValue> prefix,
            IFrequentItemsMiningParams miningParams,
            int totalTransactionsCount)
        {
            var paths = new List<IList<Tuple<TValue, double>>>();
            var valuesCounts = new Dictionary<TValue, int>();
            var minTransactionsCount = (int)totalTransactionsCount * miningParams.MinimalRelativeSupport;
            foreach (var currentNode in currentModel.HeaderTable.GetNodesForValue(valueToStartFrom))
            {
                if (currentNode.Parent == null)
                {
                    continue;
                }
                var processedNode = currentNode.Parent;
                var pathSoFar = new List<Tuple<TValue, double>>();
                while (processedNode.HasParent && !processedNode.IsRoot)
                {
                    if (prefix.ItemsSet.Contains(processedNode.Value))
                    {
                        processedNode = processedNode.Parent;
                    }
                    var copyOfNode = processedNode.CopyNode();
                    if (copyOfNode.Count > currentNode.Count)
                    {
                        copyOfNode.Count = currentNode.Count;
                    }
                    pathSoFar.Add(new Tuple<TValue, double>(copyOfNode.Value, copyOfNode.Count));
                    if (!valuesCounts.ContainsKey(copyOfNode.Value))
                    {
                        valuesCounts.Add(copyOfNode.Value, (int)copyOfNode.Count);
                    }
                    else
                    {
                        valuesCounts[copyOfNode.Value] += (int)copyOfNode.Count;
                    }
                    if (copyOfNode.Parent == null)
                    {
                        break;
                    }
                    processedNode = copyOfNode.Parent;
                }
                if (pathSoFar.Any()) paths.Add(pathSoFar);
            }
            var headerTable = new FpGrowthHeaderTable<TValue>();
            var conditionalPaths = paths
                .Select(path => path
                    .Where(el => valuesCounts[el.Item1] >= minTransactionsCount)
                    .OrderByDescending(el => valuesCounts[el.Item1])
                    .ThenByDescending(itm => itm.Item1)
                    .ToList());
            var conditionalTree = conditionalPaths
                .Aggregate(new FpGrowthNode<TValue>(),
                (tree, list) =>
                {
                    AddToTree(tree, list, headerTable.ValuesToNodesMapping);
                    return tree;
                });
            var newModel = new FpGrowthModel<TValue>(headerTable, conditionalTree);
            var nodesQueueToProcess = new Queue<FpGrowthNode<TValue>>(new[] { conditionalTree });
            while (nodesQueueToProcess.Any())
            {
                var treeNode = nodesQueueToProcess.Dequeue();
                nodesQueueToProcess = new Queue<FpGrowthNode<TValue>>(nodesQueueToProcess.Concat(treeNode.Children));
                if (treeNode.IsRoot) continue;
                var frequentItems = new FrequentItemsSet<TValue>(
                    treeNode.Count,
                    treeNode.Count / totalTransactionsCount,
                    prefix.OrderedItems.Concat(new[] { treeNode.Value }).ToArray());
                yield return frequentItems;
                var subTreesProcessingResults = ProcessPrefixTrees(treeNode.Value, newModel, frequentItems, miningParams, totalTransactionsCount);
                foreach (var result in subTreesProcessingResults)
                {
                    yield return result;
                }
                if (treeNode.IsLeaf) break;
            }
        }

        public static FpGrowthNode<TValue> BuildNewChild(
            FpGrowthNode<TValue> treeNode,
            Tuple<TValue, double> headWithCount)
        {
            var newChild = new FpGrowthNode<TValue>(headWithCount.Item1, true, headWithCount.Item2);
            treeNode.AddChild(newChild);
            return newChild;
        }

        public static void AddNodeToHeadersTable(
            FpGrowthNode<TValue> treeNode,
            IDictionary<TValue, IList<FpGrowthNode<TValue>>> headers
            )
        {
            if (headers.ContainsKey(treeNode.Value))
            {
                headers[treeNode.Value].Add(treeNode);
            }
            else
            {
                headers.Add(treeNode.Value, new List<FpGrowthNode<TValue>> { treeNode });
            }
        }
    }
}
