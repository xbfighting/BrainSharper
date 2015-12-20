using BrainSharper.Abstract.Algorithms.RuleInduction.DataStructures;
using BrainSharper.Abstract.Data;

namespace BrainSharper.Implementations.Algorithms.RuleInduction.DataStructures
{
    public struct UniversalSelector<TValue> : ISelector<TValue>
    {
        public UniversalSelector(string attributeName)
        {
            AttributeName = attributeName;
        }

        public bool IsUniversal => true;
        public bool IsEmpty => false;
        public string AttributeName { get; }

        public bool ValuesRangeOverlap(ISelector<TValue> other)
        {
            return true;
        }

        public bool Covers(IDataVector<TValue> example)
        {
            return true;
        }

        public ISelector<TValue> Intersect(ISelector<TValue> other)
        {
            return other;
        }

        public bool IsMoreGeneralThan(ISelector<TValue> other)
        {
            return true;
        }

        public bool Equals(ISelector<TValue> other)
        {
            return string.Equals(AttributeName, other.AttributeName) && other.IsUniversal;
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj))
            {
                return false;
            }
            return obj is UniversalSelector<TValue> && Equals((UniversalSelector<TValue>) obj);
        }

        public override int GetHashCode()
        {
            return (AttributeName?.GetHashCode() ?? 0) ^ true.GetHashCode();
        }
    }
}