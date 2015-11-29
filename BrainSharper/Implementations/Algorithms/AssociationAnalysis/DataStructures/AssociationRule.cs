namespace BrainSharper.Implementations.Algorithms.AssociationAnalysis.DataStructures
{
    using BrainSharper.Abstract.Algorithms.AssociationAnalysis;
    using BrainSharper.Abstract.Algorithms.AssociationAnalysis.DataStructures;

    public class AssociationRule<TValue> : IAssociationRule<TValue>
    {
        public AssociationRule(IFrequentItemsSet<TValue> antecedent, IFrequentItemsSet<TValue> consequent)
        {
            Antecedent = antecedent;
            Consequent = consequent;
        }

        public IFrequentItemsSet<TValue> Antecedent { get; }
        public IFrequentItemsSet<TValue> Consequent { get; }

        protected bool Equals(AssociationRule<TValue> other)
        {
            return Equals(this.Antecedent, other.Antecedent) && Equals(this.Consequent, other.Consequent);
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
            if (obj.GetType() != this.GetType())
            {
                return false;
            }
            return Equals((AssociationRule<TValue>)obj);
        }

        public override int GetHashCode()
        {
            unchecked
            {
                return ((Antecedent != null ? Antecedent.GetHashCode() : 0) * 397) ^ (Consequent != null ? this.Consequent.GetHashCode() : 0);
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
    }
}
