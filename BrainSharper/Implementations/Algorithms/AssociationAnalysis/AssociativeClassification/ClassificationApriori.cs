using System;
using System.Collections.Generic;
using System.Linq;
using BrainSharper.Abstract.Algorithms.AssociationAnalysis;
using BrainSharper.Abstract.Algorithms.AssociationAnalysis.AssociativeClassification;
using BrainSharper.Abstract.Algorithms.AssociationAnalysis.DataStructures;
using BrainSharper.Abstract.Algorithms.Infrastructure;
using BrainSharper.Abstract.Data;
using BrainSharper.Implementations.Algorithms.AssociationAnalysis.Apriori;
using BrainSharper.Implementations.Algorithms.AssociationAnalysis.DataStructures;
using BrainSharper.Implementations.Algorithms.AssociationAnalysis.AssociativeClassification.Heuristics;
using BrainSharper.Implementations.Algorithms.AssociationAnalysis.DataStructures.Common;


namespace BrainSharper.Implementations.Algorithms.AssociationAnalysis.AssociativeClassification
{
    public class ClassificationApriori<TValue> : AprioriAlgorithm<IDataItem<TValue>>, IPredictionModelBuilder
    {
        private static readonly string ClassificationAprioriAlgorithmRequiresAssociationminingparams = "Classification apriori algorithm requires ClassifcationAssociationMiningParams!";
        private readonly ClassificationAprioriRulesSelector<TValue> _rulesSelectionHeuristic; 

        public ClassificationApriori(
            AssocRuleMiningMinimumRequirementsChecker<IDataItem<TValue>> assocRuleMiningRequirementsChecker,
            ClassificationAprioriRulesSelector<TValue> rulesSelectionHeuristic) 
            : base(assocRuleMiningRequirementsChecker)
        {
            _rulesSelectionHeuristic = rulesSelectionHeuristic;
        }

        public IPredictionModel BuildModel(IDataFrame dataFrame, string dependentFeatureName, IModelBuilderParams additionalParams)
        {
            var assocMiningParams = additionalParams as IClassificationAssociationMiningParams;
            if (assocMiningParams == null)
            {
                throw new ArgumentException(ClassificationAprioriAlgorithmRequiresAssociationminingparams, nameof(additionalParams));
            }

            var transactionSet = dataFrame.ToAssociativeTransactionsSet<TValue>(assocMiningParams.TransactionIdFeatureName);
            var frequentItems = base.FindFrequentItems(transactionSet, assocMiningParams);
            var associationRules = base.FindAssociationRules(transactionSet, frequentItems, assocMiningParams);
            return _rulesSelectionHeuristic.BuildPredictiveRulesSet(dataFrame, transactionSet, dependentFeatureName, associationRules);
        }

        public IPredictionModel BuildModel(IDataFrame dataFrame, int dependentFeatureIndex, IModelBuilderParams additionalParams)
        {
            return this.BuildModel(dataFrame, dataFrame.ColumnNames[dependentFeatureIndex], additionalParams);
        }

        protected override bool CandidateItemMeetsCriteria(IFrequentItemsSet<IDataItem<TValue>> itm, IFrequentItemsMiningParams miningParams)
        {
            var classificationMiningParams = miningParams as IClassificationAssociationMiningParams;
            if (classificationMiningParams == null)
            {
                throw new ArgumentException(ClassificationAprioriAlgorithmRequiresAssociationminingparams, nameof(miningParams));
            }
            return base.CandidateItemMeetsCriteria(itm, miningParams);
        }

        protected override bool AssocRuleMeetsCriteria(IAssociationRule<IDataItem<TValue>> assocRule, IAssociationMiningParams miningParams)
        {
            var classifAssocParams = miningParams as IClassificationAssociationMiningParams;
            var classificationAssocRule = assocRule as IClassificationAssociationRule<TValue>;
            if (classifAssocParams == null)
            {
                throw new ArgumentException(ClassificationAprioriAlgorithmRequiresAssociationminingparams, nameof(miningParams));
            }

            if (classificationAssocRule == null)
            {
                throw new ArgumentException("Classification apriori algorithm can work only with classification assoc rules!", nameof(assocRule));
            }

            return base.AssocRuleMeetsCriteria(assocRule, miningParams) && classificationAssocRule.ClassificationConsequent.FeatureName.Equals(classifAssocParams.DependentFeatureName);
        }

        protected override AssociationRule<IDataItem<TValue>> ConstructAssocRule(
            IFrequentItemsSet<IDataItem<TValue>> currentItemSet,
            IFrequentItemsSet<IDataItem<TValue>> antecedentFrequentItemsSet,
            IFrequentItemsSet<IDataItem<TValue>> consequentFrequentItem, 
            IAssociationMiningParams assocMiningParams, 
            double confidence)
        {
            var classifAssocParams = assocMiningParams as IClassificationAssociationMiningParams;
            if (classifAssocParams == null)
            {
                throw new ArgumentException(ClassificationAprioriAlgorithmRequiresAssociationminingparams, nameof(assocMiningParams));
            }

            return new ClassificationAssociationRule<TValue>(
                antecedentFrequentItemsSet,
                consequentFrequentItem,
                currentItemSet.Support,
                currentItemSet.RelativeSupport,
                confidence);
        }

        protected override IEnumerable<IAssociationRule<IDataItem<TValue>>> ProduceAssociationRules(IFrequentItemsSet<IDataItem<TValue>> currentItemSet,
            IFrequentItemsSearchResult<IDataItem<TValue>> frequentItemsSearchResult, IAssociationMiningParams assocMiningParams)
        {
            var classificationParams = assocMiningParams as IClassificationAssociationMiningParams;
            if (currentItemSet.ItemsSet.Any(itm => itm.FeatureName.Equals(classificationParams.DependentFeatureName)))
            {
                return base.ProduceAssociationRules(currentItemSet, frequentItemsSearchResult, assocMiningParams);
            }
            return new IAssociationRule<IDataItem<TValue>>[0];
        }

        protected override IEnumerable<IEnumerable<IDataItem<TValue>>> ProduceConsequents(IFrequentItemsSet<IDataItem<TValue>> currentItemSet, IAssociationMiningParams assocMiningParams)
        {
            var classificationParams = assocMiningParams as IClassificationAssociationMiningParams;
            var classificationConsequent =
                currentItemSet.ItemsSet.First(itm => itm.FeatureName.Equals(classificationParams.DependentFeatureName));
            return new[] {new[] { classificationConsequent }};
        }
    }
}
