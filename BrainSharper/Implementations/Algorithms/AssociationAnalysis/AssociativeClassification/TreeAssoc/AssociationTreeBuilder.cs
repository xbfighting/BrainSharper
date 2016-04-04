using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using BrainSharper.Abstract.Algorithms.AssociationAnalysis;
using BrainSharper.Abstract.Algorithms.AssociationAnalysis.AssociativeClassification;
using BrainSharper.Abstract.Algorithms.AssociationAnalysis.DataStructures;
using BrainSharper.Abstract.Algorithms.Infrastructure;
using BrainSharper.Abstract.Data;
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
            return null;
        }

        public FpGrowthModel<TValue> BuildClassificationFpGrowthModel(
            ITransactionsSet<IDataItem<TValue>> transactions,
            IClassificationAssociationMiningParams classificationAsssociationMiningParams
            )
        {
            var headersDictionary = new Dictionary<IDataItem<TValue>, IList<FpGrowthNode<IDataItem<TValue>>>>();
            var treeRoot = InsertTransactionsIntoTree(transactions, classificationAsssociationMiningParams, headersDictionary);
            return null;
        }

        public FpGrowthNode<IDataItem<TValue>> InsertTransactionsIntoTree(
            ITransactionsSet<IDataItem<TValue>> transactions,
            IClassificationAssociationMiningParams classificationAsssociationMiningParams,
            Dictionary<IDataItem<TValue>, IList<FpGrowthNode<IDataItem<TValue>>>> headersDictionary)
        {
            var treeRoot = new FpGrowthNode<IDataItem<TValue>>();
            foreach (var transaction in transactions.TransactionsList)
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
                //TODO: extract class label attribute
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
            return new TransactionsSet<IDataItem<TValue>>(remappedTransactions.Where(tran => tran.TransactionItems.Any()));
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
                newNode = BuildNewNodeAndAddToTree(classLabelValue, tree, terminalNode, head);
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
                    newNode = matchingChild;
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
                        var classificationChild = BuildNodeWithInitialClassDistribution(matchingChild.Value,
                            classLabelValue, (int) matchingChild.Count + head.Item2);
                        newNode = classificationChild;
                        tree.ReplaceChild(matchingChild, classificationChild);
                    }
                }
            }
            else
            {
                newNode = BuildNewNodeAndAddToTree(classLabelValue, tree, terminalNode, head);
            }
            return new Tuple<bool, FpGrowthNode<IDataItem<TValue>>>(anyChildMatched, newNode);
        }

        protected FpGrowthNode<IDataItem<TValue>> BuildNewNodeAndAddToTree(
            TValue classLabelValue, 
            FpGrowthNode<IDataItem<TValue>> tree, 
            bool terminalNode, 
            Tuple<IDataItem<TValue>, int> head)
        {
            var newNode = terminalNode
                ? BuildNodeWithInitialClassDistribution(head.Item1, classLabelValue, head.Item2)
                : new FpGrowthNode<IDataItem<TValue>>(head.Item1, true, head.Item2);
            tree.AddChild(newNode);
            return newNode;
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