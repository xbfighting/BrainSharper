namespace BrainSharper.Abstract.Algorithms.AssociationAnalysis
{
    public interface IDissociationRulesMiningParams : IAssociationMiningParams
    {
        double MaxRelativeJoin { get; }
    }
}
