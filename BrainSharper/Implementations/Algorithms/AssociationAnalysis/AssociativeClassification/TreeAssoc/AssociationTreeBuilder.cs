using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Threading.Tasks;
using BrainSharper.Abstract.Algorithms.AssociationAnalysis;
using BrainSharper.Abstract.Algorithms.AssociationAnalysis.AssociativeClassification;
using BrainSharper.Abstract.Algorithms.AssociationAnalysis.DataStructures;
using BrainSharper.Abstract.Algorithms.Infrastructure;
using BrainSharper.Abstract.Data;
using BrainSharper.General.Utils;
using BrainSharper.Implementations.Algorithms.AssociationAnalysis.AssociativeClassification.Common;
using BrainSharper.Implementations.Algorithms.AssociationAnalysis.AssociativeClassification.TreeAssoc.Dtos;
using BrainSharper.Implementations.Algorithms.AssociationAnalysis.DataStructures.Common;
using BrainSharper.Implementations.Algorithms.AssociationAnalysis.DataStructures.FPGrowth;
using BrainSharper.Implementations.Algorithms.AssociationAnalysis.FPGrowth;

namespace BrainSharper.Implementations.Algorithms.AssociationAnalysis.AssociativeClassification.TreeAssoc
{
    public class AssociationTreeBuilder<TValue> : FpGrowthBuilder<IDataItem<TValue>>, IPredictionModelBuilder
    {
        private const string InvalidParamsMessage = "Invalid parameters for associative classification tree builder!";

        public IPredictionModel BuildModel(IDataFrame dataFrame, string dependentFeatureName, IModelBuilderParams additionalParams)
        {
            throw new NotImplementedException();
        }

        public IPredictionModel BuildModel(IDataFrame dataFrame, int dependentFeatureIndex, IModelBuilderParams additionalParams)
        {
            throw new NotImplementedException();
        }

        public override IFrequentItemsSearchResult<IDataItem<TValue>> FindFrequentItems(ITransactionsSet<IDataItem<TValue>> transactions,
            IFrequentItemsMiningParams frequentItemsMiningParams)
        {
            var classificationMiningParams = GetValidParameters(frequentItemsMiningParams);
            var initialItems = GenerateInitialFrequentItems(transactions, frequentItemsMiningParams);
            var transactonsWithOnlyFrequentElements = RemapTrasactionsToContainOnlyFrequentItems(transactions, initialItems, frequentItemsMiningParams);
            var classificationFpGrowthModel = BuildClassificationFpGrowthModel(transactonsWithOnlyFrequentElements,
                classificationMiningParams);
            var frequentItems = base.PerformFrequentItemsMining(
                initialItems.Where(kvp => kvp.Key.FeatureName != classificationMiningParams.DependentFeatureName).Select(kvp => kvp.Value).ToList(), 
                classificationFpGrowthModel,
                frequentItemsMiningParams, 
                transactions.TransactionsCount);
            return null;
        }

        public FpGrowthModel<IDataItem<TValue>> BuildClassificationFpGrowthModel(
            ITransactionsSet<IDataItem<TValue>> transactions,
            IClassificationAssociationMiningParams classificationAsssociationMiningParams
            )
        {
            var headersDictionary = new Dictionary<IDataItem<TValue>, IList<FpGrowthNode<IDataItem<TValue>>>>();
            var treeRoot = InsertTransactionsIntoTree(transactions, classificationAsssociationMiningParams, headersDictionary);
            return new FpGrowthModel<IDataItem<TValue>>(new FpGrowthHeaderTable<IDataItem<TValue>>(headersDictionary), treeRoot);
        }

        public FpGrowthNode<IDataItem<TValue>> InsertTransactionsIntoTree(
            ITransactionsSet<IDataItem<TValue>> transactions,
            IClassificationAssociationMiningParams classificationAsssociationMiningParams,
            Dictionary<IDataItem<TValue>, IList<FpGrowthNode<IDataItem<TValue>>>> headersDictionary)
        {
            var treeRoot = new FpGrowthNode<IDataItem<TValue>>();
            foreach (var transaction in transactions.TransactionsList.OrderBy(tran => tran.TransactionItems.Count()))
            {
                var classLabelItem =
                    transaction.TransactionItems.FirstOrDefault(
                        itm => itm.FeatureName.Equals(classificationAsssociationMiningParams.DependentFeatureName))
                        .FeatureValue;
                var items =
                    transaction.TransactionItems.Where(
                        itm => itm.FeatureName != classificationAsssociationMiningParams.DependentFeatureName)
                        .Select(itm => new Tuple<IDataItem<TValue>, int>(itm, 1))
                        .ToList();
                AddTransactionNodesToTree(items, classLabelItem, treeRoot, headersDictionary);
                //TODO: check preconditions
                //TODO: after preconditions are ok: insert to tree
            }
            return treeRoot;
        }

        public override ITransactionsSet<IDataItem<TValue>> RemapTrasactionsToContainOnlyFrequentItems(ITransactionsSet<IDataItem<TValue>> transactions, IDictionary<IDataItem<TValue>, IFrequentItemsSet<IDataItem<TValue>>> frequentItems,
            IFrequentItemsMiningParams miningParams)
        {
            var classificationParams = miningParams as IClassificationAssociationMiningParams;
            var remappedTransactions = new ITransaction<IDataItem<TValue>>[transactions.TransactionsCount];
            Parallel.For(0, transactions.TransactionsCount, tranIdx =>
            {
                var transaction = transactions.TransactionsList.ElementAt(tranIdx);
                var frequentElementsOnly = transaction.TransactionItems
                    .Where(frequentItems.ContainsKey)
                    .OrderByDescending(itm => frequentItems[itm].Support)
                    .ThenByDescending(itm => itm)
                    .ToList();

                // CMAR - assumption 1 - get only transactions, which contain decisive attr after remapping
                if (frequentElementsOnly.Any(itm => itm.FeatureName.Equals(classificationParams.DependentFeatureName)))
                {
                    remappedTransactions[tranIdx] = new Transaction<IDataItem<TValue>>(transaction.TransactionKey,
                        frequentElementsOnly);
                }
                else
                {
                    remappedTransactions[tranIdx] = new Transaction<IDataItem<TValue>>(transaction.TransactionKey, new IDataItem<TValue>[0]);
                }
                
            });
            var noElemes = remappedTransactions.AsParallel().Where(tran => tran.TransactionItems.Any());
            return new TransactionsSet<IDataItem<TValue>>(
                remappedTransactions
                .Where(tran => tran.TransactionItems.Any())
                .Select(tran => new ClassificationTransaction<TValue>(tran, classificationParams.DependentFeatureName)));
        }

        protected IClassificationAssociationMiningParams GetValidParameters(IFrequentItemsMiningParams frequentItemsMiningParams)
        {
            var classificationMiningParams = frequentItemsMiningParams as IClassificationAssociationMiningParams;
            if (classificationMiningParams == null)
            {
                throw new ArgumentException(InvalidParamsMessage);
            }
            return classificationMiningParams;
        }

        protected void AddTransactionNodesToTree(
            IList<Tuple<IDataItem<TValue>, int>> itemsToAdd,
            TValue classLabelValue,
            FpGrowthNode<IDataItem<TValue>> tree,
            IDictionary<IDataItem<TValue>, IList<FpGrowthNode<IDataItem<TValue>>>> headersTable)
        {
            if (!itemsToAdd.Any()) return;
            var head = itemsToAdd.First();
            var tail = itemsToAdd.Skip(1).ToList();
            FpGrowthNode<IDataItem<TValue>> newNode;
            var terminalNode = !tail.Any();
            bool anyChildMatched = false;

            if (!tree.HasChildren)
            {
                newNode = BuildNodeWithInitialClassDistribution(head.Item1, classLabelValue, head.Item2);
                tree.AddChild(newNode);
            }
            else
            {
                Tuple<bool, FpGrowthNode<IDataItem<TValue>>> result = ProcessChildren(
                    classLabelValue, 
                    tree, head, 
                    terminalNode);
                anyChildMatched = result.Item1;
                newNode = result.Item2;
            }

            if (!anyChildMatched)
            {
                if(headersTable.ContainsKey(head.Item1)) headersTable[head.Item1].Add(newNode);
                else headersTable.Add(head.Item1, new List<FpGrowthNode<IDataItem<TValue>>> { newNode } );
            }
            AddTransactionNodesToTree(tail, classLabelValue, newNode, headersTable);
        }

        private ClassificationFpGrowthNode<IDataItem<TValue>, TValue> BuildClassificationChildIfNeccessary(
            FpGrowthNode<IDataItem<TValue>> tree,
            FpGrowthNode<IDataItem<TValue>> child,
            TValue classLabelValue,
            int newCount
            )
        {
            if (child is ClassificationFpGrowthNode<IDataItem<TValue>, TValue>)
            {
                var classificationChild = child as ClassificationFpGrowthNode<IDataItem<TValue>, TValue>;
                classificationChild.AddOrIncrementClassLabelCount(classLabelValue, newCount);
                return classificationChild;
            }
            else
            {
                var classificationChild = BuildNodeWithInitialClassDistribution(child.Value,
                            classLabelValue, (int)child.Count + newCount);
                tree.ReplaceChild(child, classificationChild);
                return classificationChild;
            }
        }

        private Tuple<bool, FpGrowthNode<IDataItem<TValue>>> ProcessChildren(TValue classLabelValue, FpGrowthNode<IDataItem<TValue>> tree, Tuple<IDataItem<TValue>, int> head, bool terminalNode)
        {
            bool anyChildMatched = false;
            FpGrowthNode<IDataItem<TValue>> newNode;
            var matchingChild = tree.Children.FirstOrDefault(child => child.Value.Equals(head.Item1));
            if (matchingChild != null)
            {
                anyChildMatched = true;
                if (!terminalNode)
                {
                    newNode = BuildClassificationChildIfNeccessary(tree, matchingChild, classLabelValue, 1);
                    newNode.IncrementCountBy(head.Item2);
                }
                else
                {
                    if (matchingChild is ClassificationFpGrowthNode<IDataItem<TValue>, TValue>)
                    {
                        var classificationChild = matchingChild as ClassificationFpGrowthNode<IDataItem<TValue>, TValue>;
                        // TODO: check if this assumption is correct -should the count be always incremented by one?
                        classificationChild.AddOrIncrementClassLabelCount(classLabelValue, 1);
                        newNode = classificationChild;
                    }
                    else
                    {
                        newNode = BuildClassificationChildIfNeccessary(tree, matchingChild, classLabelValue, 1);
                        newNode.IncrementCountBy(head.Item2);
                    }
                }
            }
            else
            {
                newNode = BuildNodeWithInitialClassDistribution(head.Item1, classLabelValue, head.Item2);
                tree.AddChild(newNode);
            }
            return new Tuple<bool, FpGrowthNode<IDataItem<TValue>>>(anyChildMatched, newNode);
        }

        public override IEnumerable<IFrequentItemsSet<IDataItem<TValue>>> ProcessPrefixPathForValue(
            IDataItem<TValue> valueToStartFrom,
            FpGrowthModel<IDataItem<TValue>> currentModel,
            IFrequentItemsSet<IDataItem<TValue>> currentPrefix, 
            IFrequentItemsMiningParams miningParams, 
            int totalTransactionsCount)
        {
            var transformedPaths = new List<IEnumerable<ClassificationFpGrowthNode<IDataItem<TValue>, TValue>>>();
            double minimalTransactionsCount = totalTransactionsCount * miningParams.MinimalRelativeSupport;
            var valuesFrequenceies = new Dictionary<IDataItem<TValue>, double>();
            var classValueFrequencies = new Dictionary<TValue, int>();
            var support = 0.0;
            foreach (var node in currentModel.HeaderTable.GetNodesForValue(valueToStartFrom))
            {
                var currentClassificationNode = node as ClassificationFpGrowthNode<IDataItem<TValue>, TValue>;
                valuesFrequenceies.AddOrUpdate(currentClassificationNode.Value, currentClassificationNode.Count, (v1, v2) => v1 + v2);
                foreach (var classLabelDistr in currentClassificationNode.ClassLabelDistributions)
                {
                    if (classValueFrequencies.ContainsKey(classLabelDistr.Key))
                        classValueFrequencies[classLabelDistr.Key] += classLabelDistr.Value.Count;
                    else classValueFrequencies.Add(classLabelDistr.Key, classLabelDistr.Value.Count);
                }
                support += currentClassificationNode.Count;

                var pathToNode = currentClassificationNode.GetPathToSelf().Reverse().Cast<ClassificationFpGrowthNode<IDataItem<TValue>, TValue>>();
                var pathWithAdjustedClassesCounts = new List<ClassificationFpGrowthNode<IDataItem<TValue>, TValue>>();
                foreach (var nodeInPath in pathToNode)
                {
                    var copyOfnode = (ClassificationFpGrowthNode<IDataItem<TValue>, TValue>) nodeInPath.CopyNode(false);
                    var copyOfMasterNodeClassLabelDistributions = new Dictionary<TValue, ClassLabelCountInfo<TValue>>(currentClassificationNode.ClassLabelDistributions);
                    if (copyOfnode.Count > node.Count) copyOfnode.Count = node.Count;
                    // TODO: consider it as a performance bottleneck. It might be a problem for bigger datasets
                    copyOfnode.ClassLabelDistributions = copyOfMasterNodeClassLabelDistributions;

                    /*
                    foreach (var classLabelAndCount in copyOfnode.ClassLabelDistributions)
                    {
                        if (currentClassificationNode.ClassLabelDistributions.ContainsKey(classLabelAndCount.Key))
                        {
                            //TODO: heuristic  to select only valid classification rules
                      }
                    }
                    */

                    valuesFrequenceies.AddOrUpdate(copyOfnode.Value, copyOfnode.Count, (actual, newCount) => actual + newCount);
                    pathWithAdjustedClassesCounts.Add(copyOfnode);
                }
                transformedPaths.Add(pathWithAdjustedClassesCounts);
            }

            var extendedPrefix = new Common.ClassificationFrequentItemsSet<TValue>(
                currentPrefix.ItemsSet.Union(new List<IDataItem<TValue>> { valueToStartFrom }),
                classValueFrequencies,
                support,
                support / totalTransactionsCount
                );
            yield return extendedPrefix;

            var transformedPathsWithOnlyFrequentElems =
                transformedPaths.Select(
                    path =>
                        path.Where(node => valuesFrequenceies.ContainsKey(node.Value) && valuesFrequenceies[node.Value] >= minimalTransactionsCount)
                            .OrderByDescending(node => valuesFrequenceies[node.Value])
                            .ToList());

            var treeRoot = new ClassificationFpGrowthNode<IDataItem<TValue>, TValue>();

            var headersDictionary = new Dictionary<IDataItem<TValue>, IList<FpGrowthNode<IDataItem<TValue>>>>();
            foreach (var path in transformedPathsWithOnlyFrequentElems)
            {
                AddNodesToTree(treeRoot, path);
                foreach(var node in path) AddNodeToHeadersTable(node, headersDictionary);
            }

            // TODO: it can be refactored for better style
            var prefixModel = new FpGrowthModel<IDataItem<TValue>>(new FpGrowthHeaderTable<IDataItem<TValue>>(headersDictionary), treeRoot);
            foreach (var prefixValue in prefixModel.HeaderTable.ValuesToNodesMapping.Keys)
            {
                var subPrefixes = ProcessPrefixPathForValue(prefixValue, prefixModel, extendedPrefix, miningParams,
                    totalTransactionsCount);
                foreach (var subPrefix in subPrefixes) yield return subPrefix;
            }

            
        }

        protected void AddNodesToTree(
            ClassificationFpGrowthNode<IDataItem<TValue>, TValue> parentNode,
            IEnumerable<ClassificationFpGrowthNode<IDataItem<TValue>, TValue>> nodesToAdd)
        {
            if (!nodesToAdd.Any()) return;
            var head = nodesToAdd.First();
            ClassificationFpGrowthNode<IDataItem<TValue>, TValue> childToProcess = null;
            if (!parentNode.HasChildren)
            {
                childToProcess = ClassificationFpGrowthNode<IDataItem<TValue>, TValue>.FromOther(head);
                parentNode.AddChild(childToProcess);
            }
            else
            {
                var matchingChild = parentNode.Children.FirstOrDefault(chd => chd.Value.Equals(head.Value)) as ClassificationFpGrowthNode<IDataItem<TValue>, TValue>;
                if (matchingChild != null)
                {
                    childToProcess = matchingChild;
                    matchingChild.IncrementCountBy(head.Count);
                    foreach (var kvp in head.ClassLabelDistributions)
                    {
                        if (matchingChild.ClassLabelDistributions.ContainsKey(kvp.Key))
                            matchingChild.ClassLabelDistributions[kvp.Key].IncrementCount(kvp.Value.Count);
                        else
                            matchingChild.ClassLabelDistributions.Add(kvp.Key,
                                ClassLabelCountInfo<TValue>.FromOther(kvp.Value));
                    }
                }
                else
                {
                    childToProcess = ClassificationFpGrowthNode<IDataItem<TValue>, TValue>.FromOther(head);
                    parentNode.AddChild(childToProcess);
                }
            }
            AddNodesToTree(childToProcess, nodesToAdd.Skip(1));
        }

        protected ClassificationFpGrowthNode<IDataItem<TValue>, TValue> BuildNodeWithInitialClassDistribution(
            IDataItem<TValue> nodeValue,
            TValue classLabel,
            int count
            )
        {
            return new ClassificationFpGrowthNode<IDataItem<TValue>, TValue>(nodeValue, new Dictionary<TValue, ClassLabelCountInfo<TValue>>
            {
                [classLabel] = new ClassLabelCountInfo<TValue>(classLabel, 1)
            }, true, count);
        }

        protected override bool InitialItemMeetsCriteria(KeyValuePair<IDataItem<TValue>, IList<object>> itemWithSupport, double transactionsCount,
            IFrequentItemsMiningParams miningParams)
        {
            var classificationParams = miningParams as IClassificationAssociationMiningParams;
            return base.InitialItemMeetsCriteria(itemWithSupport, transactionsCount, miningParams) || itemWithSupport.Key.FeatureName.Equals(classificationParams.DependentFeatureName);
        }
    }
}