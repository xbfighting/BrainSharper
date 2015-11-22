namespace BrainSharper.Abstract.FeaturesEngineering.Discretization
{
    using Data;

    public interface ISupervisedClassificationDiscretizer
    {
        ISupervisedDiscretizationResult Discretize(
            IDataFrame dataFrame,
            string dependentFeatureName,
            string numericFeatureName,
            string newFeatureName);
    }
}
