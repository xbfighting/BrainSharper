using System;
using BrainSharper.Abstract.Algorithms.AssociationAnalysis.DataStructures;
using System.Collections.Generic;

namespace BrainSharper
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

