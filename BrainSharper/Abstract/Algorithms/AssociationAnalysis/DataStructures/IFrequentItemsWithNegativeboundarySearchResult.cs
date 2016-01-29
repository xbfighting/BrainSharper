using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BrainSharper.Abstract.Algorithms.AssociationAnalysis.DataStructures
{
    public interface IFrequentItemsWithNegativeboundarySearchResult<TValue> : IFrequentItemsSearchResult<TValue>
    {
        bool ContainsNegativeBoundary { get; }
        IList<IFrequentItemsSet<TValue>> NegativeBoundaryItems { get; }
        IList<IFrequentItemsSet<TValue>> GetNegativeBoundaryItemsBySize(int size);
        IList<int> NegativeBoundaryItemsSizes { get; } 
    }
}
