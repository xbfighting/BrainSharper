using System.Collections.Generic;
using BrainSharper.Abstract.Algorithms.AssociationAnalysis.AssociativeClassification;
using BrainSharper.Abstract.Algorithms.AssociationAnalysis.DataStructures;
using BrainSharper.Abstract.Data;


namespace BrainSharper.Implementations.Algorithms.AssociationAnalysis.AssociativeClassification
{
    public abstract class ClassificationAprioriRulesSelector<TValue>
    {
        public abstract IAssociativeClassificationModel<TValue> BuildPredictiveRulesSet(
            IDataFrame dataFrame,
            ITransactionsSet<IDataItem<TValue>> transactionsSet,
            string dependentFeatureName,
            IList<IAssociationRule<IDataItem<TValue>>> associationRules);
    }
}
