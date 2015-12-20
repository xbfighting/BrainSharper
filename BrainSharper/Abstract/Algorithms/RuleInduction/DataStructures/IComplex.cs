using System.Collections.Generic;
using BrainSharper.Abstract.Data;

namespace BrainSharper.Abstract.Algorithms.RuleInduction.DataStructures
{
    public interface IComplex<TValue>
    {
        IList<ISelector<TValue>> Selectors { get; }
        IList<string> CoveredAttributes { get; }
        bool IsEmpty { get; }
        bool IsUniversal { get; }
        ISelector<TValue> this[string attributeName] { get; }
        bool Covers(IDataVector<TValue> example);
        bool HasSelectorForAttribute(string attributeName);
        IComplex<TValue> Intersect(IComplex<TValue> other);
        bool IsMoreGeneralThan(IComplex<TValue> other);
        IComplex<TValue> SetNewSelector(ISelector<TValue> newSelector);
    }
}