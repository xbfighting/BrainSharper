using BrainSharper.Abstract.Algorithms.Infrastructure;

namespace BrainSharper.Abstract.Algorithms.AssociationAnalysis.AssociativeClassification
{
    public  interface IClassificationAssociationMiningParams : IAssociationMiningParams, IModelBuilderParams
    {
        string DependentFeatureName { get; }
        string TransactionIdFeatureName { get; }
    }
}
