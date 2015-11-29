namespace BrainSharper.Abstract.Algorithms.AssociationAnalysis.AssociativeClassification
{
    using DataStructures;

    using Data;

    public interface IClassificationAssociationRule<TValue> : IAssociationRule<IDataItem<TValue>>
    {
        IDataItem<TValue> ClassificationConsequent { get; }
    }
}
