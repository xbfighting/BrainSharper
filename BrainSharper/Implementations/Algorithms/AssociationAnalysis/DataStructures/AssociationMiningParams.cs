using BrainSharper.Abstract.Algorithms.AssociationAnalysis;

namespace BrainSharper.Implementations.Algorithms.AssociationAnalysis.DataStructures
{
    public struct AssociationMiningParams : IAssociationMiningParams
    {
        public AssociationMiningParams(double minimalRelativeSupport, double minimalConfidence)
        {
            MinimalRelativeSupport = minimalRelativeSupport;
            MinimalConfidence = minimalConfidence;
        }

        public double MinimalRelativeSupport { get; }
        public double MinimalConfidence { get; }
    }
}