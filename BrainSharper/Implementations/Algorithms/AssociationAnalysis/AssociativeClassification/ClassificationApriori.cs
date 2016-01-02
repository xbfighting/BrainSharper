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

namespace BrainSharper.Implementations.Algorithms.AssociationAnalysis.AssociativeClassification
{
    public class ClassificationApriori<TValue> : AprioriAlgorithm<IDataItem<TValue>>, IPredictionModelBuilder
    {
        private static readonly string ClassificationAprioriAlgorithmRequiresAssociationminingparams = "Classification apriori algorithm requires ClassifcationAssociationMiningParams!";

        public ClassificationApriori(AssocRuleMiningMinimumRequirementsChecker<IDataItem<TValue>> assocRuleMiningRequirementsChecker) 
            : base(assocRuleMiningRequirementsChecker)
        {
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
            return BuildPredictiveRulesSet(dataFrame, transactionSet, dependentFeatureName, associationRules);
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

            if (!itm.ItemsSet.Any(elem => elem.FeatureName.Equals(classificationMiningParams.DependentFeatureName)))
            {
                return false;
            }

            return base.CandidateItemMeetsCriteria(itm, miningParams);
        }

        protected IAssociativeClassificationModel<TValue> BuildPredictiveRulesSet(
            IDataFrame dataFrame,
            ITransactionsSet<IDataItem<TValue>> transactionsSet,
            string dependentFeatureName,
            IList<IAssociationRule<IDataItem<TValue>>> associationRules
            )
        {
            var sortedRules =
                associationRules
                    .Cast<IClassificationAssociationRule<TValue>>()
                    .OrderByDescending(rule => rule.Confidence)
                    .ThenByDescending(rule => rule.RelativeSuppot)
                    .ThenBy(rule => rule.Antecedent.ItemsSet.Count)
                    .ToList();

            var rulesCoverage = new List<RuleCoverageDataDto>();
            var remainingExamples = new List<int>(dataFrame.RowIndices);

            for (int ruleIdx = 0; ruleIdx < sortedRules.Count; ruleIdx++)
            {
                if (remainingExamples.Any())
                {
                    var currentRule = sortedRules[ruleIdx];
                    var ruleCoverageData = new RuleCoverageDataDto(currentRule);
                    for (int rowIdx = 0; rowIdx < dataFrame.RowCount; rowIdx++)
                    {
                        var currentRow = dataFrame.GetRowVector<TValue>(rowIdx);
                        if (currentRule.Covers(currentRow))
                        {
                            ruleCoverageData.CoveredExamples.Add(rowIdx);

                            var expectedVal = currentRow[dependentFeatureName];
                            var predictedVal = currentRule.ClassificationConsequent.FeatureValue;

                            if (expectedVal.Equals(predictedVal))
                            {
                                ruleCoverageData.IncrementCorrectClassif();
                            }
                            else
                            {
                                ruleCoverageData.IncrementIncorrectClassif();
                            }
                        }
                    }
                    if (ruleCoverageData.CoversAnyExample)
                    {
                        remainingExamples = remainingExamples.Except(ruleCoverageData.CoveredExamples).ToList();
                        var defaultClass =
                            dataFrame
                            .GetSubsetByRows(remainingExamples)
                            .GetColumnVector<TValue>(dependentFeatureName)
                            .GroupBy(val => val)
                            .OrderByDescending(grp => grp.Count())
                            .First()
                            .Key;
                        ruleCoverageData.DefaultValueForRemainingData = defaultClass;
                        rulesCoverage.Add(ruleCoverageData);
                    }
                }
            }


            return null;
            // return new AssociativeClassificationModel<TValue>(rulesCoveringAtLeastOneExample, defaultValue, dependentFeatureName);
        }

        private static void FindRulesToDelete(IDataFrame dataFrame, List<IClassificationAssociationRule<TValue>> sortedRules, List<int> rulesNotCoveringAnyRow)
        {
            for (int ruleIdx = 0; ruleIdx < sortedRules.Count; ruleIdx++)
            {
                var currentRule = sortedRules[ruleIdx];
                if (!dataFrame.RowIndices.Any(rowIdx => currentRule.Covers(dataFrame.GetRowVector<TValue>(rowIdx))))
                {
                    rulesNotCoveringAnyRow.Add(ruleIdx);
                }
            }
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
            return base.AssocRuleMeetsCriteria(assocRule, miningParams);
            /*
            return base.AssocRuleMeetsCriteria(assocRule, miningParams) && classificationAssocRule.ClassificationConsequent.FeatureName.Equals(classifAssocParams.DependentFeatureName);
            */
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
                currentItemSet.RelativeSuppot,
                confidence);
        }

        private class RuleCoverageDataDto
        {
            public RuleCoverageDataDto(IClassificationAssociationRule<TValue> rule)
            {
                Rule = rule;
                CoveredExamples = new HashSet<int>();
                CorrectClassifications = 0;
                IncorrectClassifications = 0;
                DefaultValueForRemainingData = default(TValue);
            }

            public IClassificationAssociationRule<TValue> Rule { get; } 
            public ISet<int> CoveredExamples { get; set; }
            public int CorrectClassifications { get; set; }
            public int IncorrectClassifications { get; set; }
            public TValue DefaultValueForRemainingData { get; set; }

            public double Accuracy
                => CorrectClassifications/(double) (CorrectClassifications + IncorrectClassifications);

            public bool CoversAnyExample => CoveredExamples.Any();

            public void IncrementCorrectClassif()
            {
                CorrectClassifications++;
            }

            public void IncrementIncorrectClassif()
            {
                IncorrectClassifications++;
            }
        }
    }
}
