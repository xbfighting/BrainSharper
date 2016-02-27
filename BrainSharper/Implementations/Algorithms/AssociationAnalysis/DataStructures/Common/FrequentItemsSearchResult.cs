using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using BrainSharper.Abstract.Algorithms.AssociationAnalysis.DataStructures;

namespace BrainSharper.Implementations.Algorithms.AssociationAnalysis.DataStructures.Common
{
	public class FrequentItemsSearchResult<TValue> : IFrequentItemsSearchResult<TValue>
    {
        private readonly IDictionary<int, IList<IFrequentItemsSet<TValue>>> _frequentItemsSetsBySize;

        public FrequentItemsSearchResult(IDictionary<int, IList<IFrequentItemsSet<TValue>>> frequentItemsSetsBySize)
        {
            this._frequentItemsSetsBySize = frequentItemsSetsBySize;
			FrequentItemsBySize = new ReadOnlyDictionary<int, IList<IFrequentItemsSet<TValue>>> (this._frequentItemsSetsBySize);
        }

        public IList<IFrequentItemsSet<TValue>> FrequentItems => _frequentItemsSetsBySize.Values.SelectMany(itm => itm).ToList();

		public IDictionary<int, IList<IFrequentItemsSet<TValue>>> FrequentItemsBySize { get; }

        public IList<int> FrequentItemsSizes => _frequentItemsSetsBySize.Keys.ToList();

        public IList<IFrequentItemsSet<TValue>> this[int size]
        {
            get
            {
                IList<IFrequentItemsSet<TValue>> result;
                if (_frequentItemsSetsBySize.TryGetValue(size, out result))
                {
                    return result;
                }
                return new List<IFrequentItemsSet<TValue>>();
            }
        }
    }
}
