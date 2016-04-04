using System.Collections.Generic;
using BrainSharper.Abstract.Data;
using BrainSharper.Implementations.Algorithms.AssociationAnalysis.DataStructures.Common;

namespace BrainSharper.Implementations.Algorithms.AssociationAnalysis.AssociativeClassification.TreeAssoc.Dtos
{
    public class ClassificationFrequentItemsSet<TValue> : FrequentItemsSet<IDataItem<TValue>>
    {
        public ClassificationFrequentItemsSet(
            ISet<IDataItem<TValue>> itemsSet,
            ClassLabelsDistribution<TValue> classLabelDistributions,
            double support = 0, 
            double relativeSupport = 0) : base(itemsSet, support, relativeSupport)
        {
            ClassLabelDistribution = classLabelDistributions;
        }

        public ClassificationFrequentItemsSet(
            IEnumerable<IDataItem<TValue>> itemsSet,
            ClassLabelsDistribution<TValue> classLabelDistributions,
            double support = 0, 
            double relativeSupport = 0) : base(itemsSet, support, relativeSupport)
        {
            ClassLabelDistribution = classLabelDistributions;
        }

        public ClassificationFrequentItemsSet(
            ClassLabelsDistribution<TValue> classLabelDistributions,
            double support, 
            double relativesupport, 
            params IDataItem<TValue>[] elements) : base(support, relativesupport, elements)
        {
            ClassLabelDistribution = classLabelDistributions;
        }

        public ClassificationFrequentItemsSet(
            IEnumerable<object> transactionIds, 
            IEnumerable<IDataItem<TValue>> items,
            ClassLabelsDistribution<TValue> classLabelDistributions,
            double support = 0, 
            double relativeSuppot = 0) : base(transactionIds, items, support, relativeSuppot)
        {
            ClassLabelDistribution = classLabelDistributions;
        }

        public ClassificationFrequentItemsSet(
            IEnumerable<object> transactionIds, 
            ISet<IDataItem<TValue>> itemsSet,
            ClassLabelsDistribution<TValue> classLabelDistributions,
            double support = 0, 
            double relativeSuppot = 0) : base(transactionIds, itemsSet, support, relativeSuppot)
        {
            ClassLabelDistribution = classLabelDistributions;
        }

        public ClassificationFrequentItemsSet(
            double support, 
            double relativeSuppot, 
            IEnumerable<object> transactionIds,
            ClassLabelsDistribution<TValue> classLabelDistributions,
            params IDataItem<TValue>[] items) : base(support, relativeSuppot, transactionIds, items)
        {
            ClassLabelDistribution = classLabelDistributions;
        }

        public ClassLabelsDistribution<TValue> ClassLabelDistribution { get; }
    }
}
