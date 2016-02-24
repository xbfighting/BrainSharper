using System.Collections.Generic;
using BrainSharper.Abstract.Algorithms.AssociationAnalysis.DataStructures;

namespace BrainSharper.Abstract.Algorithms.AssociationAnalysis.DissociativeRulesMining
{
	public interface IDissociativeRuleCandidateItemset<TValue>
	{
		IFrequentItemsSet<TValue> FirstSubset { get; }
		IFrequentItemsSet<TValue> SecondSubset { get; }
		ISet<TValue> AllItemsSet { get; }
		double? Support { get; set; }
		double? RelativeSupport { get; set; }
		bool IsSupportSet { get; }
		bool IsRelativeSet { get; }
	}
}

