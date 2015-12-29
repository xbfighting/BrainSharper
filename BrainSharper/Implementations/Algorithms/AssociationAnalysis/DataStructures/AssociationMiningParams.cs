using BrainSharper.Abstract.Algorithms.AssociationAnalysis;

namespace BrainSharper.Implementations.Algorithms.AssociationAnalysis.DataStructures
{
    public struct AssociationMiningParams : IAssociationMiningParams
    {
        public AssociationMiningParams(
            double minimalRelativeSupport,
            double? minimalConfidence, 
            double? minimalLift = null,
            bool allowMultiSelectorConsequent = false)
            : this()
        {
            MinimalConfidence = minimalConfidence;
            MinimalRelativeSupport = minimalRelativeSupport;
            MinimalLift = minimalLift;
            AllowMultiSelectorConsequent = allowMultiSelectorConsequent;
        }

        public double MinimalRelativeSupport { get; }

        public double? MinimalConfidence { get; }

        public double? MinimalLift { get; }

        public bool AllowMultiSelectorConsequent { get; }
    }
}
