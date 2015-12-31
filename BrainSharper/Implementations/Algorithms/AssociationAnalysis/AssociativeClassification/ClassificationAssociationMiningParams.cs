using BrainSharper.Abstract.Algorithms.AssociationAnalysis;
using BrainSharper.Abstract.Algorithms.AssociationAnalysis.AssociativeClassification;
using BrainSharper.Implementations.Algorithms.AssociationAnalysis.DataStructures;

namespace BrainSharper.Implementations.Algorithms.AssociationAnalysis.AssociativeClassification
{
    public class ClassificationAssociationMiningParams : AssociationMiningParams, IClassificationAssociationMiningParams
    {
        public ClassificationAssociationMiningParams(
            string dependentFeatureName, 
            double minimalRelativeSupport, 
            string transactionIdFeatureName,
            double? minimalConfidence, 
            double? minimalLift = null)
            : base(minimalRelativeSupport, minimalConfidence, minimalLift, false)
        {
            DependentFeatureName = dependentFeatureName;
            TransactionIdFeatureName = transactionIdFeatureName;
        }

        public string DependentFeatureName { get; }
        public string TransactionIdFeatureName { get; }

        public static IClassificationAssociationMiningParams FromAssociationMiningParams(
            IAssociationMiningParams assocMiningParams,
            string dependentFeatureName,
            string transactionIdFeatureName = null)
        {
            return new ClassificationAssociationMiningParams(
                dependentFeatureName,
                assocMiningParams.MinimalRelativeSupport, 
                transactionIdFeatureName,
                assocMiningParams.MinimalConfidence,
                assocMiningParams.MinimalLift);
        }
    }
}
