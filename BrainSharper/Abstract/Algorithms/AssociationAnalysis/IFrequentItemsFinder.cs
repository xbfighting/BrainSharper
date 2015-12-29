using System.Collections.Generic;
using BrainSharper.Abstract.Algorithms.AssociationAnalysis.DataStructures;

namespace BrainSharper.Abstract.Algorithms.AssociationAnalysis
{
    public interface IFrequentItemsFinder<TValue>
    {
        IFrequentItemsSearchResult<TValue> FindFrequentItems(
            ITransactionsSet<TValue> transactions,
            IFrequentItemsMiningParams frequentItemsMiningParams);
    }
}