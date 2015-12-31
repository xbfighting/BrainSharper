using System;
using System.Linq;
using BrainSharper.Abstract.Algorithms.AssociationAnalysis.AssociativeClassification;
using BrainSharper.Abstract.Algorithms.AssociationAnalysis.DataStructures;
using BrainSharper.Abstract.Data;
using BrainSharper.Implementations.Algorithms.AssociationAnalysis.DataStructures;

namespace BrainSharper.Implementations.Algorithms.AssociationAnalysis.AssociativeClassification
{
    public class ClassificationAssociationRule<TValue> : AssociationRule<IDataItem<TValue>>, IClassificationAssociationRule<TValue>
    {
        private static readonly string AssociativeClassificationRuleCannotHaveMoreThanConsequentItem = "Associative classification rule cannot have more than 1 consequent item";

        public ClassificationAssociationRule(
            IFrequentItemsSet<IDataItem<TValue>> antecedent, 
            IFrequentItemsSet<IDataItem<TValue>> consequent, 
            double support, 
            double relativeSupport, 
            double confidence) 
            : base(antecedent, consequent, support, relativeSupport, confidence)
        {
            if (consequent.ItemsSet.Count > 1)
            {
                throw new ArgumentException(AssociativeClassificationRuleCannotHaveMoreThanConsequentItem, nameof(consequent));
            }
            ClassificationConsequent = consequent.ItemsSet.First();
        }

        public IDataItem<TValue> ClassificationConsequent { get; }

        public bool Covers(IDataVector<TValue> row)
        {
            foreach (var antecedentElement in Antecedent.OrderedItems)
            {
                if (!row.FeatureNames.Contains(antecedentElement.FeatureName))
                {
                    return false;
                }
                var rowValue = row[antecedentElement.FeatureName];
                if (!rowValue.Equals(antecedentElement.FeatureValue))
                {
                    return false;
                }
            }
            return true;
        }
    }
}
