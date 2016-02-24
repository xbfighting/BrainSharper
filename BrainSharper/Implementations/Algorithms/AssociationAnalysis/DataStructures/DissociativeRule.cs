using BrainSharper.Abstract.Algorithms.AssociationAnalysis.DataStructures;
using BrainSharper.Abstract.Algorithms.AssociationAnalysis.DissociativeRulesMining;

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
            double @join,
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
            Join = @join;
        }

        public double Join { get; }

        protected bool Equals(DissociativeRule<TValue> other)
        {
            return base.Equals(other) && Join.Equals(other.Join);
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != this.GetType()) return false;
            return Equals((DissociativeRule<TValue>) obj);
        }

        public override int GetHashCode()
        {
            unchecked
            {
                return (base.GetHashCode()*397) ^ Join.GetHashCode();
            }
        }
    }
}
