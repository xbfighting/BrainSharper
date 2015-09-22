
namespace BrainSharper.Implementations.Algorithms.RuleBasedSystems.DataStructures
{
    using Abstract.Algorithms.RuleBasedSystems.DataStructures;
    using Abstract.Data;

    public struct UniversalSelector : ISelector
    {
        public UniversalSelector(string attributeName)
        {
            AttributeName = attributeName;
        }

        public bool IsUniversal => true;

        public bool IsEmpty => false;

        public string AttributeName { get; }

        public bool Covers<TValue>(IDataVector<TValue> example)
        {
            return true;
        }

        public ISelector Intersect(ISelector other)
        {
            return other;
        }

        public bool IsMoreDetailedThan(ISelector other)
        {
            return false;
        }

        public bool Equals(ISelector other)
        {
            return string.Equals(AttributeName, other.AttributeName) && other.IsUniversal;
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj))
            {
                return false;
            }
            return obj is UniversalSelector && Equals((UniversalSelector)obj);
        }

        public override int GetHashCode()
        {
            return (AttributeName?.GetHashCode() ?? 0) ^ true.GetHashCode();
        }
    }
}
