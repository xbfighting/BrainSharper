namespace BrainSharper.Abstract.Algorithms.AssociationAnalysis.DissociativeRulesMining
{
	public interface IDissociativeRulesMiningParams : IAssociationMiningParams
	{
		double MaxRelativeJoin { get; }
	}
}

