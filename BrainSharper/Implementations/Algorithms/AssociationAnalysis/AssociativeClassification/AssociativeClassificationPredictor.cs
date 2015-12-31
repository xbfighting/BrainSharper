using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using BrainSharper.Abstract.Algorithms.AssociationAnalysis.AssociativeClassification;
using BrainSharper.Abstract.Algorithms.Infrastructure;
using BrainSharper.Abstract.Data;

namespace BrainSharper.Implementations.Algorithms.AssociationAnalysis.AssociativeClassification
{
    public class AssociativeClassificationPredictor<TValue> : IAssociativeClassifier<TValue>
    {
        public IList<TValue> Predict(IDataFrame queryDataFrame, IPredictionModel model, string dependentFeatureName)
        {
            var associativeModel = GetAssociativeModel(model);
            return Predict(queryDataFrame, associativeModel, dependentFeatureName);
        }

        public IList<TValue> Predict(IDataFrame queryDataFrame, IPredictionModel model, int dependentFeatureIndex)
        {
            var associativeModel = GetAssociativeModel(model);
            return Predict(queryDataFrame, associativeModel, dependentFeatureIndex);
        }

        public IList<TValue> Predict(IDataFrame queryDataFrame, IAssociativeClassificationModel<TValue> model, string dependentFeatureName)
        {
            var predictions = new TValue[queryDataFrame.RowCount];
            Parallel.ForEach(queryDataFrame.RowIndices, rowIdx =>
            {
                var currentRow = queryDataFrame.GetRowVector<TValue>(rowIdx);
                var anySolutionFound = false;
                foreach (var rule in model.ClassificationRules)
                {
                    if (rule.Covers(currentRow))
                    {
                        predictions[rowIdx] = rule.ClassificationConsequent.FeatureValue;
                        anySolutionFound = true;
                        break;
                    }
                }
                if (!anySolutionFound)
                {
                    predictions[rowIdx] = model.DefaultValue;
                }
            });
            return predictions;
        }

        public IList<TValue> Predict(IDataFrame queryDataFrame, IAssociativeClassificationModel<TValue> model, int dependentFeatureIndex)
        {
            return Predict(queryDataFrame, model, queryDataFrame.ColumnNames[dependentFeatureIndex]);
        }

        private static IAssociativeClassificationModel<TValue> GetAssociativeModel(IPredictionModel predictionModel)
        {
            var associativeModel = predictionModel as IAssociativeClassificationModel<TValue>;
            if (associativeModel == null)
            {
                throw new ArgumentException("Associative classifier requires AssociativeClassificationModel!", nameof(predictionModel));
            }
            return associativeModel;
        }
    }
}
