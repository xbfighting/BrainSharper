namespace BrainSharper.Implementations.Algorithms.RuleBasedSystems.DataStructures
{
    using System.Collections.Generic;
    using System.Linq;

    using Abstract.Algorithms.RuleBasedSystems.DataStructures;
    using Abstract.Data;

    public class Complex : IComplex
    {
        private readonly IDictionary<string, ISelector> selectorsByName;

        public Complex(IList<ISelector> selectors = null)
        {
            Selectors = selectors ?? new List<ISelector>();
            CoveredAttributes = Selectors.Select(selector => selector.AttributeName).ToList();
            IsEmpty = !Selectors.Any() || Selectors.Any(sel => sel.IsEmpty);
            IsUniversal = Selectors.All(sel => sel.IsUniversal);
            selectorsByName = Selectors.ToDictionary(sel => sel.AttributeName, sel => sel);
        }

        public IList<ISelector> Selectors { get; }

        public IList<string> CoveredAttributes { get; }

        public bool IsEmpty { get; }

        public bool IsUniversal { get; }

        public ISelector this[string attributeName]
        {
            get
            {
                if (selectorsByName.ContainsKey(attributeName))
                {
                    return selectorsByName[attributeName];
                }
                return new UniversalSelector(attributeName);
            }
        } 

        public bool Covers<TValue>(IDataVector<TValue> example)
        {
            return this.Selectors.All(selector => selector.Covers(example));
        }

        public bool HasSelectorForAttribute(string attributeName)
        {
            return selectorsByName.ContainsKey(attributeName);
        }

        public IComplex Intersect(IComplex other)
        {
            if (IsEmpty || other.IsEmpty)
            {
                return new Complex();
            }

            var intersectedSelectors = new List<ISelector>();
            foreach (var selector in Selectors)
            {
                var otherSelectorForAttr = other[selector.AttributeName];
                var intersectedSelector = selector.Intersect(otherSelectorForAttr);
                intersectedSelectors.Add(intersectedSelector);
            }

            return new Complex(intersectedSelectors);
        }

        public bool IsMoreDetailedThan(IComplex other)
        {
            if (CoveredAttributes.Count != other.CoveredAttributes.Count
                || CoveredAttributes.Except(other.CoveredAttributes).Any())
            {
                return false;
            }

            return Selectors.All(selector => selector.IsMoreDetailedThan(other[selector.AttributeName]));
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
            return Equals((Complex)obj);
        }

        public override int GetHashCode()
        {
            return Selectors?.Select(sel => sel.GetHashCode()).Sum() ?? 0;
        }

        protected bool Equals(Complex other)
        {
            if (CoveredAttributes.Count != other.CoveredAttributes.Count
                || CoveredAttributes.Except(other.CoveredAttributes).Any())
            {
                return false;
            }

            return Selectors.All(selector => selector.Equals(other[selector.AttributeName]));
        }
    }
}
