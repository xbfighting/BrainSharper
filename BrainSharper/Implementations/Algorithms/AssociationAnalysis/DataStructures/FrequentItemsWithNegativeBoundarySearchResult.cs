using System.Collections.Generic;
using System.Linq;
using BrainSharper.Abstract.Algorithms.AssociationAnalysis.DataStructures;

namespace BrainSharper.Implementations.Algorithms.AssociationAnalysis.DataStructures
{
    public class FrequentItemsWithNegativeBoundarySearchResult<TValue> : FrequentItemsSearchResult<TValue>, IFrequentItemsWithNegativeboundarySearchResult<TValue>
    {
        private readonly IDictionary<int, IList<IFrequentItemsSet<TValue>>> _negativeBoundaryItemsBySize;

        public FrequentItemsWithNegativeBoundarySearchResult(IDictionary<int, IList<IFrequentItemsSet<TValue>>> frequentItemsSetsBySize, IDictionary<int, IList<IFrequentItemsSet<TValue>>> negativeBoundaryItems = null) : base(frequentItemsSetsBySize)
        {
            _negativeBoundaryItemsBySize = negativeBoundaryItems ??
                                           new Dictionary<int, IList<IFrequentItemsSet<TValue>>>();
            ContainsNegativeBoundary = negativeBoundaryItems != null;
        }

        public IList<IFrequentItemsSet<TValue>> GetNegativeBoundaryItemsBySize(int size)
        {
            return _negativeBoundaryItemsBySize[size];
        }

        public IList<int> NegativeBoundaryItemsSizes => _negativeBoundaryItemsBySize.Keys.ToList();

        public bool ContainsNegativeBoundary { get; }

        public IList<IFrequentItemsSet<TValue>> NegativeBoundaryItems
            => _negativeBoundaryItemsBySize.Values.SelectMany(itm => itm).ToList();
    }
}
