using System;
using BrainSharper.Abstract.Algorithms.AssociationAnalysis;

namespace BrainSharper.Implementations.Algorithms.AssociationAnalysis.Apriori
{
    using BrainSharper.Abstract.Algorithms.AssociationAnalysis.DataStructures;

    public delegate bool MiningMinimumRequirementsChecker<TValue>(
        IAssociationRule<TValue> assocRule,
        IAssociationMiningParams associationMiningParams);

    public delegate bool IsValueAboveThreshold(
        double value, 
        IAssociationMiningParams associationMiningParams);

    public static class AssociationMiningParamsInterpreter
    {
        public static bool AreMinimalRequirementsMet<TValue>(
            IAssociationRule<TValue> assocRule,
            IAssociationMiningParams associationMiningParams)
        {
            if (associationMiningParams.MinimalConfidence.HasValue)
            {
                if(assocRule.Confidence < associationMiningParams.MinimalConfidence)
                {
                    return false;
                }
            } else if (associationMiningParams.MinimalLift.HasValue)
            {
                if(assocRule.Confidence < associationMiningParams.MinimalLift)
                {
                    return false;
                }
            }
            else
            {
                throw new ArgumentException(
                    "Either minimal confidence or lift must be specified",
                    nameof(associationMiningParams)
                    );
            }
            return assocRule.RelativeSuppot >= associationMiningParams.MinimalRelativeSupport;
        }
    }
}
