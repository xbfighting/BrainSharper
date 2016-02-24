using System.Collections.Generic;
using System.Linq;
using BrainSharper.Abstract.Algorithms.AssociationAnalysis.DataStructures;

namespace BrainSharper.Implementations.Algorithms.AssociationAnalysis.DataStructures
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
