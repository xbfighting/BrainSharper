using System;
using System.Collections.Generic;
using System.Linq;
using BrainSharper.Abstract.Algorithms.RuleInduction.DataStructures;
using BrainSharper.Abstract.Data;

namespace BrainSharper.Implementations.Algorithms.RuleInduction.DataStructures
{
    public class DisjunctiveSelector<TValue> : Selector<TValue>, IDisjunctiveSelector<TValue>
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

        public override bool ValuesRangeOverlap(ISelector<TValue> other)
        {
            if (other.IsUniversal)
            {
                return true;
            }
            var otherDisjuncitve = other as IDisjunctiveSelector<TValue>;
            if (otherDisjuncitve == null)
            {
                return false;
            }

            if (otherDisjuncitve.AttributeName != AttributeName)
            {
                return false;
            }
            return AllowedValues.Intersect(otherDisjuncitve.AllowedValues).Any();
        }

        public override bool Covers(IDataVector<TValue> example)
        {
            if (!example.FeatureNames.Contains(AttributeName))
            {
                return false;
            }
            return AllowedValues.Contains(example[AttributeName]);
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
                throw new ArgumentException(
                    $"Cannot intersect disjunctive selector and non-disjunctive selector of type {other.GetType().Name}");
            }


            if (otherDisjunctive.AttributeName != AttributeName)
            {
                throw new ArgumentException(
                    $"Cannot intersect selectors for attributes: {AttributeName} and {other.AttributeName}");
            }

            var intersectedAllowedValues = AllowedValues.Intersect(otherDisjunctive.AllowedValues).ToList();
            if (!intersectedAllowedValues.Any())
            {
                return new EmptySelector<TValue>(AttributeName);
            }

            return new DisjunctiveSelector<TValue>(AttributeName, new HashSet<object>(intersectedAllowedValues));
        }

        public override bool IsMoreGeneralThan(ISelector<TValue> other)
        {
            if (!(other is IDisjunctiveSelector<TValue>))
            {
                return false;
            }

            if (!AttributeName.Equals(other.AttributeName))
            {
                return false;
            }

            var otherDisjunctive = (IDisjunctiveSelector<TValue>) other;
            return AllowedValues.IsSupersetOf(otherDisjunctive.AllowedValues);
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
            return Equals((IDisjunctiveSelector<TValue>) obj);
        }

        public override int GetHashCode()
        {
            unchecked
            {
                return (base.GetHashCode()*397) ^ (AllowedValues?.Aggregate(1, (i, o) => i + o.GetHashCode()) ?? 0);
            }
        }

        protected bool Equals(IDisjunctiveSelector<TValue> other)
        {
            if (other == null)
            {
                return false;
            }
            return base.Equals(other) && AllowedValues.SetEquals(other.AllowedValues);
        }
    }
}