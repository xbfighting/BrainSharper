using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BrainSharper.Implementations.Algorithms.AssociationAnalysis.AssociativeClassification.Heuristics
{
    using BrainSharper.Abstract.Algorithms.AssociationAnalysis.AssociativeClassification;
    using BrainSharper.Abstract.Algorithms.AssociationAnalysis.DataStructures;
    using BrainSharper.Abstract.Data;

    public class AccuracyBasedRulesSelector<TValue> : ClassificationAprioriRulesSelector<TValue>
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

            var rulesCoveringData = new Dictionary<int, Dictionary<int, int>>();
            var accuracies = new Dictionary<int, double>();

            for (var ruleIdx = 0; ruleIdx < sortedRules.Count; ruleIdx++)
            {
                var currentRule = sortedRules[ruleIdx];
                var ruleCoverage = new Dictionary<int, int>();
                var coversAtLeastOneExample = false;
                foreach (var rowIdx in Enumerable.Range(0, dataFrame.RowCount))
                {
                    var currentRow = dataFrame.GetRowVector<TValue>(rowIdx);
                    if (currentRule.Covers(currentRow))
                    {
                        coversAtLeastOneExample = true;
                        var expectedVal = currentRow[dependentFeatureName];
                        var predictedVal = currentRule.ClassificationConsequent.FeatureValue;
                        var isCorrect = expectedVal.Equals(predictedVal) ? 1 : 0;
                        ruleCoverage.Add(rowIdx, isCorrect);
                    }
                }
                if (coversAtLeastOneExample)
                {
                    rulesCoveringData.Add(ruleIdx, ruleCoverage);
                    accuracies.Add(ruleIdx, ruleCoverage.Values.Sum() / (double)ruleCoverage.Count);
                }
            }

            var bestRules = new List<int>();
            while (rulesCoveringData.Any())
            {
                var maxAccRule = (from ruleIdxAndAcc in accuracies
                                 where ruleIdxAndAcc.Value == accuracies.Values.Max()
                                 select ruleIdxAndAcc.Key).First();
                var bestRuleCoveredCases = rulesCoveringData[maxAccRule];

                bestRules.Add(maxAccRule);
                accuracies.Remove(maxAccRule);
                var emptyRules = new List<int>();
                foreach (var kvp in rulesCoveringData.Where(el => el.Key != maxAccRule))
                {
                    var coveredCases = kvp.Value;
                    foreach (var bestRuleCase in bestRuleCoveredCases)
                    {
                        var caseIdx = bestRuleCase.Key;
                        if (coveredCases.ContainsKey(caseIdx))
                        {
                            coveredCases.Remove(caseIdx);
                        }
                    }
                    if (!coveredCases.Any())
                    {
                        emptyRules.Add(kvp.Key);
                    }
                }

                foreach (var emptyRule in emptyRules)
                {
                    rulesCoveringData.Remove(emptyRule);
                    accuracies.Remove(emptyRule);
                }

                rulesCoveringData.Remove(maxAccRule);
                foreach (var kvp in rulesCoveringData)
                {
                    var covering = kvp.Value;

                    accuracies[kvp.Key] = covering.Values.Sum() / (double)covering.Values.Count;
                }
            }

            var defaultClass =
                            dataFrame
                            .GetColumnVector<TValue>(dependentFeatureName)
                            .GroupBy(val => val)
                            .OrderByDescending(grp => grp.Count())
                            .First()
                            .Key;
            return new AssociativeClassificationModel<TValue>(
                bestRules.Select(idx => sortedRules[idx]).ToList(),
                defaultClass,
                dependentFeatureName
                );
        }
    }
}
