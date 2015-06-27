using System.Collections.Generic;
using BrainSharper.Abstract.Data;

namespace BrainSharper.Abstract.Algorithms.Infrastructure
{
    /// <summary>
    /// This is the most generic interface representing the machine learning predictor. It takes a query
    /// data frame and existing model, and tries to perform prediction on the unseen data.
    /// </summary>
    public interface IPredictor
    {
        IList<TResult> Predict<TResult>(IDataFrame queryDataFrame, IPredictionModel model, string dependentFeatureName);
        IList<TResult> Predict<TResult>(IDataFrame queryDataFrame, IPredictionModel model, int dependentFeatureIndex);
    }
}
