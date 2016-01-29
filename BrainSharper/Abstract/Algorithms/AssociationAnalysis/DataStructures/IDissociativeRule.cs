namespace BrainSharper.Abstract.Algorithms.AssociationAnalysis.DataStructures
{
    public interface IDissociativeRule<TValue> : IAssociationRule<TValue>
    {
        double MaxRelativeJoin { get; }
    }
}
