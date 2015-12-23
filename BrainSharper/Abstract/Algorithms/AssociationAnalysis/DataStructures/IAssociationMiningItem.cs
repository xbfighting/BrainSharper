namespace BrainSharper.Abstract.Algorithms.AssociationAnalysis.DataStructures
{
    public interface IAssociationMiningItem
    {
        double Support { get; }
        double RelativeSuppot { get; }
    }
}
