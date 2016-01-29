using System.Collections.Generic;
using System.Diagnostics;

namespace BrainSharper.Abstract.Algorithms.AssociationAnalysis.DataStructures
{
    public interface IFrequentItemsSearchResult<TValue>
    {
        IList<IFrequentItemsSet<TValue>> FrequentItems { get; }
        IList<int> FrequentItemsSizes { get; }
        IList<IFrequentItemsSet<TValue>> this[int size] { get; }
    }
}