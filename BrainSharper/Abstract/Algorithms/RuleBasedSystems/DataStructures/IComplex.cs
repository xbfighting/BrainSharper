namespace BrainSharper.Abstract.Algorithms.RuleBasedSystems.DataStructures
{
    using System.Collections.Generic;
    using Data;

    public interface IComplex
    {
        IList<ISelector> Selectors { get; }
        IList<string> CoveredAttributes { get; }
        bool IsEmpty { get; }
        bool IsUniversal { get; }
        ISelector this[string attributeName] { get; }

        bool Covers<TValue>(IDataVector<TValue> example);
        bool HasSelectorForAttribute(string attributeName);
        IComplex Intersect(IComplex other);
        bool IsMoreDetailedThan(IComplex other);
    }
}
