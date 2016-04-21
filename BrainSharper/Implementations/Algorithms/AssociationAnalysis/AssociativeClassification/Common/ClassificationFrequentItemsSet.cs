using System.Collections.Generic;
using System.Linq;
using BrainSharper.Abstract.Data;
using BrainSharper.Implementations.Algorithms.AssociationAnalysis.DataStructures.Common;

namespace BrainSharper.Implementations.Algorithms.AssociationAnalysis.AssociativeClassification.Common
{
    public class ClassificationFrequentItemsSet<TValue> : FrequentItemsSet<IDataItem<TValue>>
    {
        private readonly IDictionary<TValue, int> _classLabelCoverage;

        public ClassificationFrequentItemsSet(
            ISet<IDataItem<TValue>> itemsSet, 
            IDictionary<TValue, int> classLabelCoverage, 
            double support = 0, 
            double relativeSupport = 0) : base(itemsSet, support, relativeSupport)
        {
            _classLabelCoverage = classLabelCoverage;
        }

        public ClassificationFrequentItemsSet(
            IEnumerable<IDataItem<TValue>> itemsSet,
            IDictionary<TValue, int> classLabelCoverage,
            double support = 0, 
            double relativeSupport = 0) : base(itemsSet, support, relativeSupport)
        {
            _classLabelCoverage = classLabelCoverage;
        }

        public ClassificationFrequentItemsSet(
            double support, 
            double relativesupport,
            IDictionary<TValue, int> classLabelCoverage,
            params IDataItem<TValue>[] elements) : base(support, relativesupport, elements)
        {
            _classLabelCoverage = classLabelCoverage;
        }

        public ClassificationFrequentItemsSet(
            IEnumerable<object> transactionIds, 
            IEnumerable<IDataItem<TValue>> items,
            IDictionary<TValue, int> classLabelCoverage,
            double support = 0, 
            double relativeSuppot = 0) : base(transactionIds, items, support, relativeSuppot)
        {
            _classLabelCoverage = classLabelCoverage;
        }

        public ClassificationFrequentItemsSet(
            IEnumerable<object> transactionIds, 
            ISet<IDataItem<TValue>> itemsSet,
            IDictionary<TValue, int> classLabelCoverage,
            double support = 0, 
            double relativeSuppot = 0) : base(transactionIds, itemsSet, support, relativeSuppot)
        {
            _classLabelCoverage = classLabelCoverage;
        }

        public ClassificationFrequentItemsSet(
            double support, 
            double relativeSuppot, 
            IEnumerable<object> transactionIds,
            IDictionary<TValue, int> classLabelCoverage,
            params IDataItem<TValue>[] items) : base(support, relativeSuppot, transactionIds, items)
        {
            _classLabelCoverage = classLabelCoverage;
        }

        public IList<TValue> CoveredClassLabels => _classLabelCoverage.Keys.ToList();

        public bool CoversClassLabel(TValue classLabel)
        {
            return _classLabelCoverage.ContainsKey(classLabel);
        }

        public int GetClassLabelCount(TValue classLabel)
        {
            return _classLabelCoverage[classLabel];
        }
    }
}
