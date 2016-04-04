using System.Collections.Generic;
using BrainSharper.Abstract.Algorithms.AssociationAnalysis.AssociativeClassification;

namespace BrainSharper.Implementations.Algorithms.AssociationAnalysis.AssociativeClassification.Common
{
    public class AssociativeClassificationModel<TValue> : IAssociativeClassificationModel<TValue>
    {
        public AssociativeClassificationModel(
            IList<IClassificationAssociationRule<TValue>> classificationRules, 
            TValue defaultValue, 
            string dependentFeatureName)
        {
            ClassificationRules = classificationRules;
            DefaultValue = defaultValue;
            DependentFeatureName = dependentFeatureName;
        }

        public IList<IClassificationAssociationRule<TValue>> ClassificationRules { get; }
        public TValue DefaultValue { get; }
        public string DependentFeatureName { get; }
    }
}
