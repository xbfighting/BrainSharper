using System.Collections.Generic;
using System.Linq;

using BrainSharper.Abstract.Algorithms.AssociationAnalysis.AssociativeClassification;
using BrainSharper.Abstract.Algorithms.AssociationAnalysis.DataStructures;
using BrainSharper.Abstract.Data;

namespace BrainSharper.Implementations.Algorithms.AssociationAnalysis.AssociativeClassification.Heuristics
{
    public class BasicCARRulesSelector<TValue> : ClassificationAprioriRulesSelector<TValue>
    {
        public override IAssociativeClassificationModel<TValue> BuildPredictiveRulesSet(
            IDataFrame dataFrame,
            ITransactionsSet<IDataItem<TValue>> transactionsSet,
            string dependentFeatureName,
            IList<IAssociationRule<IDataItem<TValue>>> associationRules)
        {
            var sortedRules =
                associationRules
                    .Cast<IClassificationAssociationRule<TValue>>()
                    .OrderByDescending(rule => rule.Confidence)
                    .ThenByDescending(rule => rule.RelativeSuppot)
                    .ThenBy(rule => rule.Antecedent.ItemsSet.Count)
                    .ToList();

            var rulesCoverage = new List<RuleCoverageDataDto<TValue>>();
            var remainingExamples = Enumerable.Range(0, dataFrame.RowCount).ToList();

            for (int ruleIdx = 0; ruleIdx < sortedRules.Count; ruleIdx++)
            {
                if (remainingExamples.Any())
                {
                    var currentRule = sortedRules[ruleIdx];
                    var ruleCoverageData = new RuleCoverageDataDto<TValue>(currentRule);
                    foreach (var rowIdx in Enumerable.Range(0, dataFrame.RowCount))
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
                        if (ruleIdx > 0)
                        {
                            var previousRuleCoverageData = rulesCoverage[ruleIdx - 1];
                            if (ruleCoverageData.Accuracy < previousRuleCoverageData.Accuracy)
                            {
                                break;
                            }
                            else
                            {
                                var newRemainingExamples = remainingExamples.Except(ruleCoverageData.CoveredExamples).ToList();
                                var collectionToSelectDefaultClass = newRemainingExamples.Any()
                                    ? newRemainingExamples
                                    : Enumerable.Range(0, dataFrame.RowCount).ToList();
                                var defaultClass =
                                    dataFrame
                                    .GetSubsetByRows(collectionToSelectDefaultClass)
                                    .GetColumnVector<TValue>(dependentFeatureName)
                                    .GroupBy(val => val)
                                    .OrderByDescending(grp => grp.Count())
                                    .First()
                                    .Key;
                                ruleCoverageData.DefaultValueForRemainingData = defaultClass;
                                remainingExamples = newRemainingExamples;
                            }
                        }
                        rulesCoverage.Add(ruleCoverageData);
                    }
                }
            }

            return new AssociativeClassificationModel<TValue>
                (rulesCoverage.Select(r => r.Rule).ToList(),
                rulesCoverage.Last().DefaultValueForRemainingData,
                dependentFeatureName);
        }
    }
}
