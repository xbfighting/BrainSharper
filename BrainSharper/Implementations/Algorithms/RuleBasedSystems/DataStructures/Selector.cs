namespace BrainSharper.Implementations.Algorithms.RuleBasedSystems.DataStructures
{
    using Abstract.Algorithms.RuleBasedSystems.DataStructures;
    using Abstract.Data;

    public abstract class Selector : ISelector
    {
        protected Selector(string attributeName)
        {
            AttributeName = attributeName;
        }

        public abstract bool IsUniversal { get; }

        public abstract bool IsEmpty { get; }

        public string AttributeName { get; }

        public abstract bool Covers<TValue>(IDataVector<TValue> example);

        public abstract ISelector Intersect(ISelector other);

        public abstract bool IsMoreDetailedThan(ISelector other);

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
            return Equals((Selector)obj);
        }

        public override int GetHashCode()
        {
            return AttributeName?.GetHashCode() ?? 0;
        }

        protected bool Equals(Selector other)
        {
            return string.Equals(AttributeName, other.AttributeName);
        }

    }
}
