namespace BrainSharper.Abstract.Algorithms.AssociationAnalysis
{
    public interface IDissociationRulesMiningParams : IFrequentItemsMiningParams
    {
        double MaxRelativeJoin { get; }
    }
}
