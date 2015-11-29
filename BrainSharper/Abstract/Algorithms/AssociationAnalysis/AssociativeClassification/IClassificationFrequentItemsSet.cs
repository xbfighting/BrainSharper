namespace BrainSharper.Abstract.Algorithms.AssociationAnalysis.AssociativeClassification
{
    using BrainSharper.Abstract.Algorithms.AssociationAnalysis.DataStructures;

    using Data;

    public interface IClassificationFrequentItem<TValue> : IFrequentItemsSet<IDataItem<TValue>>
    {
    }
}
