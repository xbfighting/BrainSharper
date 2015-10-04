namespace BrainSharper.Implementations.Algorithms.RuleInduction.DataStructures
{
    using System.Collections.Generic;
    using System.Linq;

    using BrainSharper.Abstract.Algorithms.RuleInduction.DataStructures;
    using BrainSharper.Abstract.Data;
    using BrainSharper.General.Utils;

    public class Complex<TValue> : IComplex<TValue>
    {
        private readonly IDictionary<string, ISelector<TValue>> selectorsByName;

        public Complex(IList<ISelector<TValue>> selectors = null, bool isEmpty = false)
        {
            this.IsEmpty = selectors?.Any(sel => sel.IsEmpty) ?? isEmpty;
            if (!this.IsEmpty)
            {
                this.Selectors = selectors ?? new List<ISelector<TValue>>();
                this.CoveredAttributes = this.Selectors.Select(selector => selector.AttributeName).ToList();
                this.IsUniversal = this.Selectors.All(sel => sel.IsUniversal);
                this.selectorsByName = this.Selectors.ToDictionary(sel => sel.AttributeName, sel => sel);
            }
        }

        public Complex(params ISelector<TValue>[] selectors)
            : this(selectors, selectors.Any())
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
                if (this.selectorsByName.ContainsKey(attributeName))
                {
                    return this.selectorsByName[attributeName];
                }
                return new UniversalSelector<TValue>(attributeName);
            }
        } 

        public bool Covers(IDataVector<TValue> example)
        {
            return this.Selectors.All(selector => selector.Covers(example));
        }

        public bool HasSelectorForAttribute(string attributeName)
        {
            return this.selectorsByName.ContainsKey(attributeName);
        }

        public IComplex<TValue> Intersect(IComplex<TValue> other)
        {
            if (this.IsEmpty || other.IsEmpty)
            {
                return new Complex<TValue>(isEmpty: true);
            }

            if (this.IsUniversal && other.IsUniversal)
            {
                return new Complex<TValue>();
            }

            if (this.IsUniversal)
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
            foreach (var selector in this.Selectors)
            {
                var otherSelectorForAttr = other[selector.AttributeName];
                var intersectedSelector = selector.Intersect(otherSelectorForAttr);
                intersectedSelectors.Add(intersectedSelector);
            }

            return new Complex<TValue>(intersectedSelectors);
        }

        public bool IsMoreGeneralThan(IComplex<TValue> other)
        {
            if (this.IsUniversal && !other.IsUniversal)
            {
                return true;
            }

            if (this.IsUniversal && other.IsUniversal)
            {
                return false;
            }

            if (this.CoveredAttributes.Count != other.CoveredAttributes.Count
                || this.CoveredAttributes.Except(other.CoveredAttributes).Any())
            {
                return true;
            }

            return this.Selectors.All(selector => selector.IsMoreGeneralThan(other[selector.AttributeName]));
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
            if (obj.GetType() != this.GetType())
            {
                return false;
            }
            return this.Equals((Complex<TValue>)obj);
        }

        public override int GetHashCode()
        {
            return this.Selectors?.Select(sel => sel.GetHashCode()).Sum() ?? 0;
        }

        protected bool Equals(Complex<TValue> other)
        {
            if (other.IsEmpty && this.IsEmpty)
            {
                return true;
            }

            if (other.IsUniversal && this.IsUniversal)
            {
                return true;
            }

            if (!this.CoveredAttributes.IsEquivalentTo(other.CoveredAttributes))
            {
                return false;
            }

            return this.Selectors.All(selector => selector.Equals(other[selector.AttributeName]));
        }
    }
}