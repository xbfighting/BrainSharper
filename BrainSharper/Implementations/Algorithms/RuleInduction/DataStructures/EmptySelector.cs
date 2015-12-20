﻿using BrainSharper.Abstract.Algorithms.RuleInduction.DataStructures;
using BrainSharper.Abstract.Data;

namespace BrainSharper.Implementations.Algorithms.RuleInduction.DataStructures
{
    public struct EmptySelector<TValue> : ISelector<TValue>
    {
        public EmptySelector(string attributeName)
            : this()
        {
            AttributeName = attributeName;
        }

        public bool IsUniversal => false;
        public bool IsEmpty => true;
        public string AttributeName { get; }

        public bool ValuesRangeOverlap(ISelector<TValue> other)
        {
            return false;
        }

        public bool Covers(IDataVector<TValue> example)
        {
            return false;
        }

        public ISelector<TValue> Intersect(ISelector<TValue> other)
        {
            return this;
        }

        public bool IsMoreGeneralThan(ISelector<TValue> other)
        {
            return false;
        }

        public bool Equals(EmptySelector<TValue> other)
        {
            return string.Equals(AttributeName, other.AttributeName) && other.IsEmpty;
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj))
            {
                return false;
            }
            return obj is EmptySelector<TValue> && Equals((EmptySelector<TValue>) obj);
        }

        public override int GetHashCode()
        {
            return (AttributeName != null ? AttributeName.GetHashCode() : 0) + false.GetHashCode();
        }
    }
}