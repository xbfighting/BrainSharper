namespace BrainSharper.Implementations.FeaturesEngineering.Discretization
{
    using System.Collections.Generic;

    using Abstract.FeaturesEngineering;
    using Abstract.FeaturesEngineering.Discretization;

    public struct SupervisedClassificationResult : ISupervisedDiscretizationResult
    {
        public SupervisedClassificationResult(string newAttributeName, IDictionary<IRange, string> newValuesMapping)
        {
            this.NewAttributeName = newAttributeName;
            this.NewValuesMapping = newValuesMapping;
        }

        public string NewAttributeName { get; }
        public IDictionary<IRange, string> NewValuesMapping { get; }
    }
}
