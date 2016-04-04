using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using BrainSharper.Abstract.Algorithms.AssociationAnalysis.AssociativeClassification;
using BrainSharper.Abstract.Algorithms.AssociationAnalysis.DataStructures;
using BrainSharper.Abstract.Algorithms.DecisionTrees.DataStructures;
using BrainSharper.Abstract.Algorithms.DecisionTrees.Processors;
using BrainSharper.Abstract.Data;
using BrainSharper.Implementations.Algorithms.AssociationAnalysis.AssociativeClassification.Common;
using BrainSharper.Implementations.Algorithms.DecisionTrees.DataStructures;
using BrainSharper.Implementations.Algorithms.DecisionTrees.DataStructures.BinaryDecisionTrees;

namespace BrainSharper.Implementations.Algorithms.AssociationAnalysis.AssociativeClassification.Heuristics
{
    public class InformationGainRulesSelector<TValue> : ClassificationAprioriRulesSelector<TValue>
    {
        private readonly ICategoricalSplitQualityChecker _categoricalSplitQualityChecker;

        public InformationGainRulesSelector(ICategoricalSplitQualityChecker categoricalSplitQualityChecker)
        {
            _categoricalSplitQualityChecker = categoricalSplitQualityChecker;
        }

        public override IAssociativeClassificationModel<TValue> BuildPredictiveRulesSet(IDataFrame dataFrame, ITransactionsSet<IDataItem<TValue>> transactionsSet,
            string dependentFeatureName, IList<IAssociationRule<IDataItem<TValue>>> associationRules)
        {
            double initialEntorpy = _categoricalSplitQualityChecker.GetInitialEntropy(dataFrame, dependentFeatureName);
            var selectedRules = new List<RuleCoverageDataDto<TValue>>();
            //TODO: [Assoc classif] Later change it to some fancy dto, change dictionary to list maybe
            var rulesCoverageData = new Dictionary<int, Tuple<double, RuleCoverageDataDto<TValue>>>();
            var rulesIndices = Enumerable.Range(0, associationRules.Count).ToList();
            var remainingIndices = Enumerable.Range(0, dataFrame.RowCount).ToList();
            var bestAccuracySoFar = double.MinValue;
            var lowestAccuracySoFar = double.MaxValue;
            var lowestAccuracyIndex = -1;

            while (remainingIndices.Any() && rulesIndices.Any())
            {
               // Parallel.ForEach(rulesIndices, ruleIdx =>
               foreach(var ruleIdx in rulesIndices)
                {
                    var classificationRule = associationRules[ruleIdx] as IClassificationAssociationRule<TValue>;
                    var coverageData = new RuleCoverageDataDto<TValue>(classificationRule);
                    var coveredExamples = new HashSet<int>();
                    var correctClassifications = 0;
                    var incorrectClassifications = 0;
                    foreach (var rowIdx in remainingIndices)
                    {
                        var currentRow = dataFrame.GetRowVector<TValue>(rowIdx);
                        if (classificationRule.Covers(currentRow))
                        {
                            var expectedValue = currentRow[dependentFeatureName];
                            var rulePredictionValue = classificationRule.ClassificationConsequent.FeatureValue;
                            if (expectedValue.Equals(rulePredictionValue))
                            {
                                coverageData.IncrementCorrectClassif();
                            }
                            else
                            {
                                coverageData.IncrementIncorrectClassif();
                            }
                            coveredExamples.Add(rowIdx);
                        }
                    }
                    if (coveredExamples.Any())
                    {
                        var notCoveredIndices = remainingIndices.Except(coveredExamples).ToList();
                        coverageData.CoveredExamples = coveredExamples;
                        var coveredDataFrame =
                            dataFrame
                                .GetSubsetByRows(coveredExamples.ToList());

                        var nonCoveredDataFrame = dataFrame.GetSubsetByRows(notCoveredIndices);
                        var splitResult = new ISplittedData[]
                        {
                        new SplittedData(
                            new BinaryDecisionTreeLink(coveredExamples.Count/(double) remainingIndices.Count,
                                coveredExamples.Count, true),
                            coveredDataFrame),
                        new SplittedData(
                            new BinaryDecisionTreeLink(notCoveredIndices.Count/(double) remainingIndices.Count,
                                notCoveredIndices.Count, true),
                            nonCoveredDataFrame)
                        };
                        var splitQuality = _categoricalSplitQualityChecker.CalculateSplitQuality(initialEntorpy,
                            remainingIndices.Count, splitResult, dependentFeatureName);
                        //TODO: [Assoc classif] Later change it to some fancy dto
                        rulesCoverageData[ruleIdx] = new Tuple<double, RuleCoverageDataDto<TValue>>(splitQuality, coverageData);
                    }
                }//);

                if (!rulesCoverageData.Any())
                {
                    break;
                }
                var sortedRules = rulesCoverageData.Where(kvp => rulesIndices.Contains(kvp.Key)).OrderByDescending(kvp => kvp.Value.Item1);
                var bestRule = sortedRules.First();
                if (bestRule.Value.Item2.Accuracy < lowestAccuracySoFar)
                {
                    lowestAccuracySoFar = bestRule.Value.Item2.Accuracy;
                    lowestAccuracyIndex = selectedRules.Count;
                }
                
                remainingIndices = remainingIndices.Except(bestRule.Value.Item2.CoveredExamples).ToList();
                var indicesToSelectDefault = remainingIndices.Any()
                    ? remainingIndices
                    : Enumerable.Range(0, dataFrame.RowCount).ToList();
                var defaultClass =
                                   dataFrame
                                   .GetSubsetByRows(indicesToSelectDefault)
                                   .GetColumnVector<TValue>(dependentFeatureName)
                                   .GroupBy(val => val)
                                   .OrderByDescending(grp => grp.Count())
                                   .First()
                                   .Key;

                bestRule.Value.Item2.DefaultValueForRemainingData = defaultClass;
                selectedRules.Add(bestRule.Value.Item2);
                rulesIndices.Remove(bestRule.Key);
                rulesCoverageData.Remove(bestRule.Key);
            }

            var defaultClassInTheEnd = selectedRules.Last().DefaultValueForRemainingData;
            var rulesToTake = selectedRules;
            if (lowestAccuracyIndex > 0)
            {
                var cutpoint = lowestAccuracyIndex;
                rulesToTake = selectedRules.Take(cutpoint).ToList();
                defaultClassInTheEnd = rulesToTake.Last().DefaultValueForRemainingData;
            }
            var model = new AssociativeClassificationModel<TValue>(rulesToTake.Select(r => r.Rule).ToList(), defaultClassInTheEnd, dependentFeatureName);
            return model;
        }
    }
}
