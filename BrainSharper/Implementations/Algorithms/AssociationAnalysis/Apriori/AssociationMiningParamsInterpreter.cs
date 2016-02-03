using System;
using BrainSharper.Abstract.Algorithms.AssociationAnalysis;
using BrainSharper.Abstract.Algorithms.AssociationAnalysis.DataStructures;

namespace BrainSharper.Implementations.Algorithms.AssociationAnalysis.Apriori
{
    public delegate bool AssocRuleMiningMinimumRequirementsChecker<TValue>(
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
            }

            if (associationMiningParams.MinimalLift.HasValue)
            {
                if(assocRule.Lift < associationMiningParams.MinimalLift)
                {
                    return false;
                }
            }

            if(!associationMiningParams.MinimalConfidence.HasValue && !associationMiningParams.MinimalLift.HasValue)
            {
                throw new ArgumentException(
                    "Either minimal confidence or lift must be specified",
                    nameof(associationMiningParams)
                    );
            }
            return assocRule.RelativeSupport >= associationMiningParams.MinimalRelativeSupport;
        }
    }
}
