﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BrainSharper.Abstract.Algorithms.AssociationAnalysis.DataStructures
{
    public interface IFrequentItemsWithDissociativeSets<TValue> : IFrequentItemsSearchResult<TValue>
    {
		IList<IDissociativeRuleCandidateItemset<TValue>> ValidDissociativeSets { get; }
		IList<IDissociativeRuleCandidateItemset<TValue>> CandidateDissociativeSets { get; }
    }
}
