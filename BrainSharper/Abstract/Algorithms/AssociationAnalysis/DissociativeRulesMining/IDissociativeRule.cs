using System;
using BrainSharper.Abstract.Algorithms.AssociationAnalysis.DataStructures;

namespace BrainSharper
{
	public interface IDissociativeRule<TValue> : IAssociationRule<TValue>
	{
		IFrequentItemsSet<TValue> Antecedent { get; }
		IFrequentItemsSet<TValue> Consequent { get; }
		bool IsAntecedentNegated { get; }
		bool IsConsequentNegated { get; }
	}
}

