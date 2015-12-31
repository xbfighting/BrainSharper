using System.Collections.Generic;
using BrainSharper.Abstract.Algorithms.Infrastructure;

namespace BrainSharper.Abstract.Algorithms.AssociationAnalysis.AssociativeClassification
{
    public interface IAssociativeClassificationModel<TValue> : IPredictionModel
    {
        IList<IClassificationAssociationRule<TValue>> ClassificationRules { get; }
        TValue DefaultValue { get; }
        string DependentFeatureName { get; }
    }
}
