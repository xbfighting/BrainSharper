using BrainSharper.Abstract.Algorithms.AssociationAnalysis;

namespace BrainSharper.Implementations.Algorithms.AssociationAnalysis.DataStructures
{
    public struct DisociativeRulesMiningParams : IDissociationRulesMiningParams
    {
        public DisociativeRulesMiningParams(double minimalRelativeSupport, double maxRelativeJoin)
        {
            MinimalRelativeSupport = minimalRelativeSupport;
            MaxRelativeJoin = maxRelativeJoin;
        }

        public double MinimalRelativeSupport { get; }
        public double MaxRelativeJoin { get; }
    }
}
