using BrainSharper.Abstract.Algorithms.AssociationAnalysis.DataStructures;
using BrainSharper.Abstract.Data;

namespace BrainSharper.Abstract.Algorithms.AssociationAnalysis.AssociativeClassification
{
    public interface IClassificationAssociationRule<TValue> : IAssociationRule<IDataItem<TValue>>
    {
        IDataItem<TValue> ClassificationConsequent { get; }
    }
}