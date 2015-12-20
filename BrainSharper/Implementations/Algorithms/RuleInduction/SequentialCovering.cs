using System;
using System.Collections.Generic;
using System.Linq;
using BrainSharper.Abstract.Algorithms.Infrastructure;
using BrainSharper.Abstract.Algorithms.RuleInduction;
using BrainSharper.Abstract.Algorithms.RuleInduction.DataStructures;
using BrainSharper.Abstract.Data;
using BrainSharper.Implementations.Algorithms.RuleInduction.DataStructures;
using BrainSharper.Implementations.Data;

namespace BrainSharper.Implementations.Algorithms.RuleInduction
{
    public abstract class SequentialCovering<TValue> : IRulesInductionModelBuilder
    {
        public IPredictionModel BuildModel(IDataFrame dataFrame, string dependentFeatureName,
            IModelBuilderParams additionalParams)
        {
            ValidateParameters(additionalParams);

            var ruleInductionParams = (IRuleInductionParams<TValue>) additionalParams;

            var rulesList = new List<IRule<TValue>>();
            IDataItem<TValue> defaultValue = new DataItem<TValue>(dependentFeatureName, default(TValue));

            var coveredExamplesIndices = new List<int>();
            var remainingExamples = new List<int>(dataFrame.RowIndices);
            while (remainingExamples.Any())
            {
                var bestAntecedentData = FindBestRuleAntecedent(
                    dataFrame,
                    dependentFeatureName,
                    coveredExamplesIndices,
                    remainingExamples,
                    ruleInductionParams);
                var bestAntecedentComplex = bestAntecedentData.Item1;
                if (bestAntecedentComplex == null || bestAntecedentComplex.IsUniversal)
                {
                    defaultValue = FindConsequent(dataFrame, dependentFeatureName, remainingExamples);
                    break;
                }
                var bestAntecedentCoveredExamples = bestAntecedentData.Item2;
                var ruleConsequent = FindConsequent(dataFrame, dependentFeatureName, bestAntecedentCoveredExamples);
                var rule = new Rule<TValue>(new[] {bestAntecedentComplex}, ruleConsequent);
                rulesList.Add(rule);
                remainingExamples = remainingExamples.Except(bestAntecedentCoveredExamples).ToList();
            }

            if (defaultValue.FeatureValue == null)
            {
                defaultValue = FindConsequent(dataFrame, dependentFeatureName, dataFrame.RowIndices);
            }

            return new RulesList<TValue>(rulesList, defaultValue);
        }

        public IPredictionModel BuildModel(IDataFrame dataFrame, int dependentFeatureIndex,
            IModelBuilderParams additionalParams)
        {
            return BuildModel(dataFrame, dataFrame.ColumnNames[dependentFeatureIndex], additionalParams);
        }

        //TODO: refactor parameters to encapsulate in one struct
        protected abstract Tuple<IComplex<TValue>, IList<int>> FindBestRuleAntecedent(
            IDataFrame dataFrame,
            string dependentFeatureName,
            IList<int> coveredExamples,
            IList<int> remainingExamples,
            IRuleInductionParams<TValue> additionalParams);

        protected virtual IDataItem<TValue> FindConsequent(
            IDataFrame dataFrame,
            string dependentFeatureName,
            IList<int> examplesCoveredByComplex)
        {
            var majorityVote =
                dataFrame.GetSubsetByRows(examplesCoveredByComplex, true)
                    .GetColumnVector<TValue>(dependentFeatureName)
                    .Values.GroupBy(val => val)
                    .OrderByDescending(grp => grp.Count())
                    .First()
                    .Key;
            return new DataItem<TValue>(dependentFeatureName, majorityVote);
        }

        protected void ValidateParameters(IModelBuilderParams modelBuilderParams)
        {
            if (!(modelBuilderParams is IRuleInductionParams<TValue>))
            {
                throw new ArgumentException("Invalid model builder params passed to rule induction algorithm!");
            }
        }
    }
}