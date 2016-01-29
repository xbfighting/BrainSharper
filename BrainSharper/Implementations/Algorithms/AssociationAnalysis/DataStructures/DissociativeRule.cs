using BrainSharper.Abstract.Algorithms.AssociationAnalysis.DataStructures;

namespace BrainSharper.Implementations.Algorithms.AssociationAnalysis.DataStructures
{
    public class DissociativeRule<TValue> : AssociationRule<TValue>, IDissociativeRule<TValue>
    {
        public DissociativeRule(
            IFrequentItemsSet<TValue> antecedent, 
            IFrequentItemsSet<TValue> consequent, 
            double support, 
            double relativeSupport, 
            double confidence, 
            bool isAntecedentNegated = false, 
            bool isConsequentNegated = false)
            : base(
                  antecedent, 
                  consequent, 
                  support, 
                  relativeSupport, 
                  confidence, 
                  isAntecedentNegated, 
                  isConsequentNegated)
        {
        }

        public double MaxRelativeJoin => RelativeSupport;
    }
}
