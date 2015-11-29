namespace BrainSharper.Abstract.Algorithms.AssociationAnalysis
{
    public interface IAssociationMiningParams
    {
        double MinimalSupport { get; }
        double MinimalConfidence { get; }
    }
}
