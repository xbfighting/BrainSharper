using BrainSharper.Abstract.Algorithms.AssociationAnalysis;

namespace BrainSharper.Implementations.Algorithms.AssociationAnalysis.DataStructures
{
    public class DisociativeRulesMiningParams : AssociationMiningParams, IDissociationRulesMiningParams
    {
        public DisociativeRulesMiningParams(double minimalRelativeSupport, double maxRelativeJoin, double? minimalConfidence, double? minimalLift = null, bool allowMultiSelectorConsequent = false)
            : base(minimalRelativeSupport, minimalConfidence, minimalLift, allowMultiSelectorConsequent)
        {
            MaxRelativeJoin = maxRelativeJoin;
        }

        public double MaxRelativeJoin { get; }
    }
}
