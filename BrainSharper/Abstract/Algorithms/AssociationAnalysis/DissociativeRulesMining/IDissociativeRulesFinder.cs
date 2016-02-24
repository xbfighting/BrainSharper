using System;
using System.Collections.Generic;
using BrainSharper.Abstract.Algorithms.AssociationAnalysis.DataStructures;

namespace BrainSharper
{
	public interface IDissociativeRulesFinder<TValue>
	{
		IList<IDissociativeRule<TValue>> FindDissociativeRules(
			ITransactionsSet<TValue> transactionsSet,
			IDissociativeRulesMiningParams dissociativeRulesMiningParams);
	}
}

