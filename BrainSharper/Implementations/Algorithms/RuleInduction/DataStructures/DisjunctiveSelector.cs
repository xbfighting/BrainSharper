namespace BrainSharper.Implementations.Algorithms.RuleInduction.DataStructures
{
    using System;
    using System.Collections.Generic;
    using System.Linq;

    using BrainSharper.Abstract.Algorithms.RuleInduction.DataStructures;
    using BrainSharper.Abstract.Data;

    public class DisjunctiveSelector<TValue> : Selector<TValue>, IDisjunctiveSelector<TValue>
    {
        public DisjunctiveSelector(string attributeName, ISet<object> allowedValues)
            : base(attributeName)
        {
            this.AllowedValues = allowedValues;
        }

        public DisjunctiveSelector(string attributeName, params object[] allowedValues)
            : this(attributeName, new HashSet<object>(allowedValues))
        {
        }

        public override bool IsUniversal => false;

        public override bool IsEmpty => false;

        public ISet<object> AllowedValues { get; }

        public override bool Covers(IDataVector<TValue> example)
        {
            if (!example.FeatureNames.Contains(this.AttributeName))
            {
                return false;
            }
            return this.AllowedValues.Contains(example[this.AttributeName]);
        }

        public override ISelector<TValue> Intersect(ISelector<TValue> other)
        {
            if (other.IsUniversal)
            {
                return this;
            }

            if (other.IsEmpty)
            {
                return other;
            }

            var otherDisjunctive = other as IDisjunctiveSelector<TValue>;
            if (otherDisjunctive == null)
            {
                throw new ArgumentException($"Cannot intersect disjunctive selector and non-disjunctive selector of type {other.GetType().Name}");
            }

            
            if (otherDisjunctive.AttributeName != this.AttributeName)
            {
                throw new ArgumentException($"Cannot intersect selectors for attributes: {this.AttributeName} and {other.AttributeName}");
            }

            var intersectedAllowedValues = this.AllowedValues.Intersect(otherDisjunctive.AllowedValues).ToList();
            if (!intersectedAllowedValues.Any())
            {
                return new EmptySelector<TValue>(this.AttributeName);
            }

            return new DisjunctiveSelector<TValue>(this.AttributeName, new HashSet<object>(intersectedAllowedValues));
        }

        public override bool IsMoreGeneralThan(ISelector<TValue> other)
        {
            if (!(other is IDisjunctiveSelector<TValue>))
            {
                return false;
            }

            if (!this.AttributeName.Equals(other.AttributeName))
            {
                return false;
            }

            var otherDisjunctive = (IDisjunctiveSelector<TValue>)other;
            return this.AllowedValues.IsSupersetOf(otherDisjunctive.AllowedValues);
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
            return this.Equals((IDisjunctiveSelector<TValue>)obj);
        }

        public override int GetHashCode()
        {
            unchecked
            {
                return (base.GetHashCode() * 397) ^ (this.AllowedValues?.Aggregate(1, (i, o) => i + o.GetHashCode()) ?? 0);
            }
        }

        protected bool Equals(IDisjunctiveSelector<TValue> other)
        {
            if (other == null)
            {
                return false;
            }
            return base.Equals(other) && this.AllowedValues.SetEquals(other.AllowedValues);
        }
    }
}