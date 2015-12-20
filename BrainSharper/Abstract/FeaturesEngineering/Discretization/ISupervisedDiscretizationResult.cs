using System.Collections.Generic;

namespace BrainSharper.Abstract.FeaturesEngineering.Discretization
{
    public interface ISupervisedDiscretizationResult
    {
        string NewAttributeName { get; }
        IDictionary<IRange, string> NewValuesMapping { get; }
    }
}