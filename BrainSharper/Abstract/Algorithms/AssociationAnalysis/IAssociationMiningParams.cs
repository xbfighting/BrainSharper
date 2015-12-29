namespace BrainSharper.Abstract.Algorithms.AssociationAnalysis
{
    public interface IAssociationMiningParams : IFrequentItemsMiningParams
    {
        double? MinimalConfidence { get; }
        double? MinimalLift { get; }
        bool AllowMultiSelectorConsequent { get; }
    }
}
