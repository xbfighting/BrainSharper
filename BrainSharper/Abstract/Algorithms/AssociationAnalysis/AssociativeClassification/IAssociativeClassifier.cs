using System.Collections.Generic;
using BrainSharper.Abstract.Algorithms.Infrastructure;
using BrainSharper.Abstract.Data;

namespace BrainSharper.Abstract.Algorithms.AssociationAnalysis.AssociativeClassification
{
    public interface IAssociativeClassifier<TValue> : IPredictor<TValue>
    {
        IList<TValue> Predict(
            IDataFrame queryDataFrame, 
            IAssociativeClassificationModel<TValue> model, 
            string dependentFeatureName);

        IList<TValue> Predict(
            IDataFrame queryDataFrame, 
            IAssociativeClassificationModel<TValue>  model, 
            int dependentFeatureIndex);
    }
}
