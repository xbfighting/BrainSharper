using System.Collections.Generic;
using BrainSharper.Abstract.Algorithms.AssociationAnalysis.DataStructures;
using BrainSharper.Abstract.Algorithms.AssociationAnalysis.DissociativeRulesMining;

namespace BrainSharper.Implementations.Algorithms.AssociationAnalysis.DataStructures.Common
{
	public class FrequentItemsWithDissociativeSets<TValue> : FrequentItemsSearchResult<TValue>, IFrequentItemsWithDissociativeSets<TValue>
	{
		private readonly IList<IDissociativeRuleCandidateItemset<TValue>> _validDissociativeSets;
		private readonly IList<IDissociativeRuleCandidateItemset<TValue>> _candidateDissociativeSets;


		public FrequentItemsWithDissociativeSets (
			IDictionary<int, IList<IFrequentItemsSet<TValue>>> frequentItemsSetsBySize, 
			IList<IDissociativeRuleCandidateItemset<TValue>> validDissociativeSets,
			IList<IDissociativeRuleCandidateItemset<TValue>> candidateDissociativeSets
		) : base (frequentItemsSetsBySize)
		{
			_validDissociativeSets = validDissociativeSets;
			_candidateDissociativeSets = candidateDissociativeSets;
		}


		public IList<IDissociativeRuleCandidateItemset<TValue>> ValidDissociativeSets => _validDissociativeSets;
		public IList<IDissociativeRuleCandidateItemset<TValue>> CandidateDissociativeSets => _candidateDissociativeSets;
	}
}
