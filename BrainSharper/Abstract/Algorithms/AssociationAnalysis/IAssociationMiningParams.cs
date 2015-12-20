namespace BrainSharper.Abstract.Algorithms.AssociationAnalysis
{
    public interface IAssociationMiningParams
    {
        double MinimalRelativeSupport { get; }
        double MinimalConfidence { get; }
    }
}