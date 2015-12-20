﻿using System.Collections.Generic;
using BrainSharper.Abstract.Algorithms.AssociationAnalysis.DataStructures;

namespace BrainSharper.Abstract.Algorithms.AssociationAnalysis
{
    public interface IAssociationRulesFinder<TValue>
    {
        IList<IAssociationRule<TValue>> FindAssociationRules(
            ITransactionsSet<TValue> trainsactionSet,
            IFrequentItemsSearchResult<TValue> frequentItemsSearchResult,
            IAssociationMiningParams associationMiningParams
            );
    }
}
