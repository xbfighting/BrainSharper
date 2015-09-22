namespace BrainSharper.Implementations.Algorithms.RuleBasedSystems.DataStructures
{
    using Abstract.Algorithms.RuleBasedSystems.DataStructures;
    using Abstract.Data;

    public struct EmptySelector : ISelector
    {
        public EmptySelector(string attributeName)
            : this()
        {
            AttributeName = attributeName;
        }

        public bool IsUniversal => false;

        public bool IsEmpty => true;

        public string AttributeName { get; }

        public bool Covers<TValue>(IDataVector<TValue> example)
        {
            return false;
        }

        public ISelector Intersect(ISelector other)
        {
            return this;
        }

        public bool IsMoreDetailedThan(ISelector other)
        {
            if (other.IsEmpty)
            {
                return false;
            }

            return true;
        }

        public bool Equals(EmptySelector other)
        {
            return string.Equals(this.AttributeName, other.AttributeName) && other.IsEmpty;
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj))
            {
                return false;
            }
            return obj is EmptySelector && Equals((EmptySelector)obj);
        }

        public override int GetHashCode()
        {
            return (this.AttributeName != null ? AttributeName.GetHashCode() : 0) + false.GetHashCode();
        }
    }
}
