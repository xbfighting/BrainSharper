using System.Collections.Generic;
using BrainSharper.Abstract.Algorithms.AssociationAnalysis.DataStructures;

namespace BrainSharper.Abstract.Algorithms.AssociationAnalysis
{
    public interface IFrequentItemsFinder<TValue>
    {
        IList<IFrequentItemsSet<TValue>> FindFrequentItems(ITransactionsSet<TValue> transactions,
            IAssociationMiningParams associationMiningParams);
    }
}