using System;
using System.Collections.Generic;
using System.Linq;
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

    public delegate bool ItemsetValidityChecker<TValue>(
        IFrequentItemsSet<TValue> itemsSet,
        IDictionary<int, IList<IFrequentItemsSet<TValue>>> itemsBySize,
        IFrequentItemsMiningParams miningPatams);

    public static class AssociationMiningParamsInterpreter
    {
        public static bool IsItemSupportAboveThreshold<TValue>(
            IFrequentItemsSet<TValue> itemsSet,
            IDictionary<int, IList<IFrequentItemsSet<TValue>>> itemsBySize,
            IFrequentItemsMiningParams miningParams)
        {
            return itemsSet.RelativeSupport >= miningParams.MinimalRelativeSupport;
        }

        public static bool CheckIfItemIsNotCrossSupportPattern<TValue>(
            IFrequentItemsSet<TValue> itemsSet,
             IDictionary<int, IList<IFrequentItemsSet<TValue>>> itemsBySize,
            IFrequentItemsMiningParams miningParams)
        {
            var partialSupports =
                itemsSet.ItemsSet.Select(
                    itm => itemsBySize[1].First(freqItems => freqItems.ItemsSet.Contains(itm)).Support);
            var supportRatio = partialSupports.Min()/partialSupports.Max();
            return supportRatio >= miningParams.MinimalRelativeSupport;
        }

        public static bool AreMinimalRequirementsMet<TValue>(
            IAssociationRule<TValue> assocRule,
            IAssociationMiningParams associationMiningParams)
        {
            if (associationMiningParams.MinimalConfidence.HasValue)
            {
                if (assocRule.Confidence < associationMiningParams.MinimalConfidence)
                {
                    return false;
                }
            }

            if (associationMiningParams.MinimalLift.HasValue)
            {
                if (assocRule.Lift < associationMiningParams.MinimalLift)
                {
                    return false;
                }
            }

            if (!associationMiningParams.MinimalConfidence.HasValue && !associationMiningParams.MinimalLift.HasValue)
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
