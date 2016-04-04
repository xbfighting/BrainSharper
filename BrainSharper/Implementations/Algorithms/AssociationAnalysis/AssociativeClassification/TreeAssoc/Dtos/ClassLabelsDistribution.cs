using System;
using System.Collections.Generic;

namespace BrainSharper.Implementations.Algorithms.AssociationAnalysis.AssociativeClassification.TreeAssoc.Dtos
{
    public struct ClassLabelsDistribution<TValue>
    {
        private readonly IDictionary<TValue, ClassLabelCountInfo<TValue>> _distributionTable;

        public ClassLabelsDistribution(IDictionary<TValue, ClassLabelCountInfo<TValue>> distributionTable)
        {
            _distributionTable = distributionTable;
        }

        public ICollection<TValue> ClassLabels => _distributionTable.Keys;
        public ICollection<ClassLabelCountInfo<TValue>> ClassLabelCounts => _distributionTable.Values;

        public bool HasLabel(TValue label) => _distributionTable.ContainsKey(label);

        public ClassLabelCountInfo<TValue> GetClassLabelCountInfoForLabel(TValue label)
        {
            return HasLabel(label) ? _distributionTable[label] : null;
        }

    }
}