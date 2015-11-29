namespace BrainSharper.Implementations.Algorithms.AssociationAnalysis.DataStructures
{
    using BrainSharper.Abstract.Algorithms.AssociationAnalysis;

    public struct AssociationMiningParams : IAssociationMiningParams
    {
        public AssociationMiningParams(double minimalSupport, double minimalConfidence)
        {
            this.MinimalSupport = minimalSupport;
            this.MinimalConfidence = minimalConfidence;
        }

        public double MinimalSupport { get; }
        public double MinimalConfidence { get; }
    }
}
