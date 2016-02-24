using System;
using BrainSharper.Abstract.Algorithms.AssociationAnalysis;

namespace BrainSharper
{
	public interface IDissociativeRulesMiningParams : IAssociationMiningParams
	{
		double MaxRelativeJoin { get; }
	}
}

