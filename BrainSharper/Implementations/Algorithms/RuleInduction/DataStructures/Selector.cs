namespace BrainSharper.Implementations.Algorithms.RuleInduction.DataStructures
{
    using Abstract.Algorithms.RuleInduction.DataStructures;
    using Abstract.Data;

    public abstract class Selector<TValue> : ISelector<TValue>
    {
        protected Selector(string attributeName)
        {
            this.AttributeName = attributeName;
        }

        public abstract bool IsUniversal { get; }

        public abstract bool IsEmpty { get; }

        public string AttributeName { get; }

        public abstract bool ValuesRangeOverlap(ISelector<TValue> other);

        public abstract bool Covers(IDataVector<TValue> example);

        public abstract ISelector<TValue> Intersect(ISelector<TValue> other);

        public abstract bool IsMoreGeneralThan(ISelector<TValue> other);

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
            return this.Equals((Selector<TValue>)obj);
        }

        public override int GetHashCode()
        {
            return this.AttributeName?.GetHashCode() ?? 0;
        }

        protected bool Equals(Selector<TValue> other)
        {
            return string.Equals(this.AttributeName, other.AttributeName);
        }
    }
}