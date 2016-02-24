using BrainSharper.Abstract.Algorithms.AssociationAnalysis.DataStructures;

namespace BrainSharper.Implementations.Algorithms.AssociationAnalysis.DataStructures
{
	public class AssociationRule<TValue> : IDissociativeRule<TValue>
    {
        public AssociationRule(
            IFrequentItemsSet<TValue> antecedent, 
            IFrequentItemsSet<TValue> consequent,
            double support,
            double relativeSupport,
            double confidence,
            bool isAntecedentNegated = false,
            bool isConsequentNegated = false)
        {
            Antecedent = antecedent;
            Consequent = consequent;
            Support = support;
            Confidence = confidence;
            Lift = relativeSupport/(antecedent.RelativeSupport*consequent.RelativeSupport);
            RelativeSupport = relativeSupport;
            IsAntecedentNegated = isAntecedentNegated;
            IsConsequentNegated = isConsequentNegated;
        }

        public IFrequentItemsSet<TValue> Antecedent { get; }
        public IFrequentItemsSet<TValue> Consequent { get; }
        public bool IsAntecedentNegated { get; }
        public bool IsConsequentNegated { get; }
        public double Support { get; }
        public double RelativeSupport { get; }
        public double Confidence { get; }
        public double Lift { get; }

        protected bool Equals(AssociationRule<TValue> other)
        {
            return Equals(Antecedent, other.Antecedent) && Equals(Consequent, other.Consequent) && Equals(Support, other.Support) && Equals(RelativeSupport, other.RelativeSupport) && Equals(Confidence, other.Confidence) && IsAntecedentNegated.Equals(other.IsAntecedentNegated) && IsConsequentNegated.Equals(other.IsConsequentNegated);
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj))
            {
                return false;
            }
            if (ReferenceEquals(this, obj))
            {
                return true;
            }
            if (obj.GetType() != GetType())
            {
                return false;
            }
            return Equals((AssociationRule<TValue>) obj);
        }

        public override int GetHashCode()
        {
            unchecked
            {
                return ((Antecedent != null ? Antecedent.GetHashCode() : 0)*397) ^
                       (Consequent != null ? Consequent.GetHashCode() : 0) ^
                       (Support.GetHashCode()) ^
                       (RelativeSupport.GetHashCode()) ^
                       (IsAntecedentNegated.GetHashCode()) ^
                       (IsConsequentNegated.GetHashCode()) ^
                       (Confidence.GetHashCode());
            }
        }

        public static bool operator ==(AssociationRule<TValue> left, AssociationRule<TValue> right)
        {
            return Equals(left, right);
        }

        public static bool operator !=(AssociationRule<TValue> left, AssociationRule<TValue> right)
        {
            return !Equals(left, right);
        }

        public override string ToString()
        {
            return $"[{Antecedent}] => [{Consequent}] Support: {RelativeSupport}, Confidence: {Confidence}";
        }
    }
}