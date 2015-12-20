using System.Collections.Generic;
using System.Linq;
using BrainSharper.Abstract.Algorithms.RuleInduction.DataStructures;
using BrainSharper.Abstract.Data;
using BrainSharper.General.Utils;

namespace BrainSharper.Implementations.Algorithms.RuleInduction.DataStructures
{
    public class Complex<TValue> : IComplex<TValue>
    {
        private readonly IDictionary<string, ISelector<TValue>> selectorsByName;

        public Complex(IList<ISelector<TValue>> selectors = null, bool isEmpty = false)
        {
            IsEmpty = selectors?.Any(sel => sel.IsEmpty) ?? isEmpty;
            if (!IsEmpty)
            {
                Selectors = selectors ?? new List<ISelector<TValue>>();
                CoveredAttributes = Selectors.Select(selector => selector.AttributeName).ToList();
                IsUniversal = Selectors.All(sel => sel.IsUniversal);
                selectorsByName = Selectors.ToDictionary(sel => sel.AttributeName, sel => sel);
            }
        }

        public Complex(bool isEmpty = false, params ISelector<TValue>[] selectors)
            : this(selectors, isEmpty)
        {
        }

        public IList<ISelector<TValue>> Selectors { get; }
        public IList<string> CoveredAttributes { get; }
        public bool IsEmpty { get; }
        public bool IsUniversal { get; }

        public ISelector<TValue> this[string attributeName]
        {
            get
            {
                if (selectorsByName.ContainsKey(attributeName))
                {
                    return selectorsByName[attributeName];
                }
                return new UniversalSelector<TValue>(attributeName);
            }
        }

        public bool Covers(IDataVector<TValue> example)
        {
            return Selectors.All(selector => selector.Covers(example));
        }

        public bool HasSelectorForAttribute(string attributeName)
        {
            return selectorsByName.ContainsKey(attributeName);
        }

        public IComplex<TValue> Intersect(IComplex<TValue> other)
        {
            if (IsEmpty || other.IsEmpty)
            {
                return new Complex<TValue>(isEmpty: true);
            }

            if (IsUniversal && other.IsUniversal)
            {
                return new Complex<TValue>();
            }

            if (IsUniversal)
            {
                return other;
            }

            var intersectedSelectors = new List<ISelector<TValue>>();
            //TODO: !!! AAA - fix error with intersecting universal selectors!!!
            var notCoveredAttributes = other.CoveredAttributes.Except(CoveredAttributes);
            if (notCoveredAttributes.Any())
            {
                foreach (var notCoveredAttr in notCoveredAttributes)
                {
                    intersectedSelectors.Add(other[notCoveredAttr]);
                }
            }
            foreach (var selector in Selectors)
            {
                var otherSelectorForAttr = other[selector.AttributeName];
                var intersectedSelector = selector.Intersect(otherSelectorForAttr);
                intersectedSelectors.Add(intersectedSelector);
            }

            return new Complex<TValue>(intersectedSelectors);
        }

        public bool IsMoreGeneralThan(IComplex<TValue> other)
        {
            if (IsUniversal && !other.IsUniversal)
            {
                return true;
            }

            if (IsUniversal && other.IsUniversal)
            {
                return false;
            }

            if (CoveredAttributes.Count != other.CoveredAttributes.Count
                || CoveredAttributes.Except(other.CoveredAttributes).Any())
            {
                return true;
            }

            return Selectors.All(selector => selector.IsMoreGeneralThan(other[selector.AttributeName]));
        }

        public IComplex<TValue> SetNewSelector(ISelector<TValue> newSelector)
        {
            var newSelectorsByName = new Dictionary<string, ISelector<TValue>>(selectorsByName);
            if (!newSelectorsByName.ContainsKey(newSelector.AttributeName))
            {
                newSelectorsByName.Add(newSelector.AttributeName, null);
            }
            newSelectorsByName[newSelector.AttributeName] = newSelector;
            return new Complex<TValue>(
                newSelectorsByName.Values.ToList(),
                IsEmpty
                );
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
            return Equals((Complex<TValue>) obj);
        }

        public override int GetHashCode()
        {
            return Selectors?.Select(sel => sel.GetHashCode()).Sum() ?? 0;
        }

        protected bool Equals(Complex<TValue> other)
        {
            if (other.IsEmpty && IsEmpty)
            {
                return true;
            }

            if (other.IsUniversal && IsUniversal)
            {
                return true;
            }

            if (!CoveredAttributes.IsEquivalentTo(other.CoveredAttributes))
            {
                return false;
            }

            return Selectors.All(selector => selector.Equals(other[selector.AttributeName]));
        }
    }
}