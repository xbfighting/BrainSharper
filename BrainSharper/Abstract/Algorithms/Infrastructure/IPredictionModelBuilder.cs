namespace BrainSharper.Abstract.Algorithms.Infrastructure
{
    using BrainSharper.Abstract.Data;

    /// <summary>
    /// The most generic abstraction for predication algorithm. Takes the data frame and depended variable (y). Returns very generic
    /// prediction model.
    /// </summary>
    public interface IPredictionModelBuilder
    {
        IPredictionModel BuildModel(IDataFrame dataFrame, string dependentFeatureName, IModelBuilderParams additionalParams);
        IPredictionModel BuildModel(IDataFrame dataFrame, int dependentFeatureIndex, IModelBuilderParams additionalParams);
    }
}