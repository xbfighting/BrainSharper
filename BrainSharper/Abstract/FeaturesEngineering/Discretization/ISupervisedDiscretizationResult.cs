namespace BrainSharper.Abstract.FeaturesEngineering.Discretization
{
    using System.Collections.Generic;

    public interface ISupervisedDiscretizationResult
    {
        string NewAttributeName { get; }
        IDictionary<IRange, string> NewValuesMapping { get; }
    }
}
