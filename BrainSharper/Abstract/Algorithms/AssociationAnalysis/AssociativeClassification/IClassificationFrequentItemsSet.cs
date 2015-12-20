using BrainSharper.Abstract.Algorithms.AssociationAnalysis.DataStructures;
using BrainSharper.Abstract.Data;

namespace BrainSharper.Abstract.Algorithms.AssociationAnalysis.AssociativeClassification
{
    public interface IClassificationFrequentItem<TValue> : IFrequentItemsSet<IDataItem<TValue>>
    {
    }
}