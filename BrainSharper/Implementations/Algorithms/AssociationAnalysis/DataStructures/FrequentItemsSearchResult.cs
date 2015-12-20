using System.Collections.Generic;
using System.Linq;
using BrainSharper.Abstract.Algorithms.AssociationAnalysis.DataStructures;

namespace BrainSharper.Implementations.Algorithms.AssociationAnalysis.DataStructures
{
    public class FrequentItemsSearchResult<TValue> : IFrequentItemsSearchResult<TValue>
    {
        private readonly IDictionary<int, IList<IFrequentItemsSet<TValue>>> _frequentItemsSetsBySize;

        public FrequentItemsSearchResult(IDictionary<int, IList<IFrequentItemsSet<TValue>>> frequentItemsSetsBySize)
        {
            this._frequentItemsSetsBySize = frequentItemsSetsBySize;
        }

        public IList<IFrequentItemsSet<TValue>> FrequentItems => _frequentItemsSetsBySize.Values.SelectMany(itm => itm).ToList();

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
