using BrainSharper.Abstract.Algorithms.AssociationAnalysis;

namespace BrainSharper.Implementations.Algorithms.AssociationAnalysis.DataStructures
{
    public struct FrequentItemsMiningParams : IFrequentItemsMiningParams
    {
        public FrequentItemsMiningParams(double minimalRelativeSupport, double minimalConfidence, bool allowMultiSelectorConsequent = false)
        {
            MinimalRelativeSupport = minimalRelativeSupport;
        }

        public double MinimalRelativeSupport { get; }
    }
}