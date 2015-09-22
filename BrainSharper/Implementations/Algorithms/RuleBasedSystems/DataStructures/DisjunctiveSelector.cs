namespace BrainSharper.Implementations.Algorithms.RuleBasedSystems.DataStructures
{
    using System;
    using System.Collections.Generic;
    using System.Linq;

    using Abstract.Algorithms.RuleBasedSystems.DataStructures;
    using Abstract.Data;

    public class DisjunctiveSelector : Selector, IDisjunctiveSelector
    {
        public DisjunctiveSelector(string attributeName, ISet<object> allowedValues)
            : base(attributeName)
        {
            AllowedValues = allowedValues;
        }

        public DisjunctiveSelector(string attributeName, params object[] allowedValues)
            : this(attributeName, new HashSet<object>(allowedValues))
        {
        }

        public override bool IsUniversal => false;

        public override bool IsEmpty => false;

        public ISet<object> AllowedValues { get; }

        public override bool Covers<TValue>(IDataVector<TValue> example)
        {
            if (!example.FeatureNames.Contains(AttributeName))
            {
                return false;
            }
            return AllowedValues.Contains(example[AttributeName]);
        }

        public override ISelector Intersect(ISelector other)
        {
            if (other.IsUniversal)
            {
                return this;
            }

            if (other.IsEmpty)
            {
                return other;
            }

            var otherDisjunctive = other as IDisjunctiveSelector;
            if (otherDisjunctive == null)
            {
                throw new ArgumentException($"Cannot intersect disjunctive selector and non-disjunctive selector of type {other.GetType().Name}");
            }

            
            if (otherDisjunctive.AttributeName != AttributeName)
            {
                throw new ArgumentException($"Cannot intersect selectors for attributes: {AttributeName} and {other.AttributeName}");
            }

            var intersectedAllowedValues = AllowedValues.Intersect(otherDisjunctive.AllowedValues);
            if (!intersectedAllowedValues.Any())
            {
                return new EmptySelector(AttributeName);
            }

            return new DisjunctiveSelector(AttributeName, new HashSet<object>(intersectedAllowedValues));
        }

        public override bool IsMoreDetailedThan(ISelector other)
        {
            if (!(other is IDisjunctiveSelector))
            {
                return false;
            }

            if (!AttributeName.Equals(other.AttributeName))
            {
                return false;
            }

            var otherDisjunctive = (IDisjunctiveSelector)other;
            return AllowedValues.IsSubsetOf(otherDisjunctive.AllowedValues);
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
            return Equals((IDisjunctiveSelector)obj);
        }

        public override int GetHashCode()
        {
            unchecked
            {
                return (base.GetHashCode() * 397) ^ (this.AllowedValues?.Aggregate(1, (i, o) => i + o.GetHashCode()) ?? 0);
            }
        }

        protected bool Equals(IDisjunctiveSelector other)
        {
            if (other == null)
            {
                return false;
            }
            return base.Equals(other) && AllowedValues.SetEquals(other.AllowedValues);
        }
    }
}
