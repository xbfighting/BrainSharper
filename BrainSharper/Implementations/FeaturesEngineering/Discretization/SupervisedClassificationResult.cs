using System.Collections.Generic;
using BrainSharper.Abstract.FeaturesEngineering;
using BrainSharper.Abstract.FeaturesEngineering.Discretization;

namespace BrainSharper.Implementations.FeaturesEngineering.Discretization
{
    public struct SupervisedClassificationResult : ISupervisedDiscretizationResult
    {
        public SupervisedClassificationResult(string newAttributeName, IDictionary<IRange, string> newValuesMapping)
        {
            NewAttributeName = newAttributeName;
            NewValuesMapping = newValuesMapping;
        }

        public string NewAttributeName { get; }
        public IDictionary<IRange, string> NewValuesMapping { get; }
    }
}