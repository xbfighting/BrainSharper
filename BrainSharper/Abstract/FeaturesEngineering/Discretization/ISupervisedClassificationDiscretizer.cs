using BrainSharper.Abstract.Data;

namespace BrainSharper.Abstract.FeaturesEngineering.Discretization
{
    public interface ISupervisedClassificationDiscretizer
    {
        ISupervisedDiscretizationResult Discretize(
            IDataFrame dataFrame,
            string dependentFeatureName,
            string numericFeatureName,
            string newFeatureName);
    }
}