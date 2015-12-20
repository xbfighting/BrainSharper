using System;
using System.Collections.Generic;
using System.Linq;
using BrainSharper.Abstract.Algorithms.Infrastructure;
using BrainSharper.Abstract.Algorithms.RuleInduction.DataStructures;
using BrainSharper.Abstract.Data;

namespace BrainSharper.Implementations.Algorithms.RuleInduction
{
    public class RulesPredictor<TValue> : IPredictor<TValue>
    {
        public IList<TValue> Predict(IDataFrame queryDataFrame, IPredictionModel model, string dependentFeatureName)
        {
            if (!(model is IRulesList<TValue>))
            {
                throw new ArgumentException("Invalid model type passed for RulesPredictor!");
            }
            var rulesListModel = model as IRulesList<TValue>;
            var predictions = new List<TValue>();
            foreach (var rowIdx in queryDataFrame.RowIndices)
            {
                var row = queryDataFrame.GetRowVector<TValue>(rowIdx, true);
                var wasCoveredByAnyRule = false;
                foreach (var rule in rulesListModel.Rules)
                {
                    if (rule.AntecedentLogicOperator == LogicOperator.And)
                    {
                        if (rule.Antecedents.All(antecedent => antecedent.Covers(row)))
                        {
                            var consequent = rule.Consequent.FeatureValue;
                            predictions.Add(consequent);
                            wasCoveredByAnyRule = true;
                            break;
                        }
                    }
                    else
                    {
                        if (rule.Antecedents.Any(antecedent => antecedent.Covers(row)))
                        {
                            var consequent = rule.Consequent.FeatureValue;
                            predictions.Add(consequent);
                            wasCoveredByAnyRule = true;
                            break;
                        }
                    }
                }

                if (!wasCoveredByAnyRule)
                {
                    predictions.Add(rulesListModel.Default.FeatureValue);
                }
            }
            return predictions;
        }

        public IList<TValue> Predict(IDataFrame queryDataFrame, IPredictionModel model, int dependentFeatureIndex)
        {
            return Predict(queryDataFrame, model, queryDataFrame.ColumnNames[dependentFeatureIndex]);
        }
    }
}