using BrainSharper.Abstract.Data;

namespace BrainSharper.Abstract.Algorithms.RuleInduction.DataStructures
{
    public interface ISelector<TValue>
    {
        // TODO: !!! AAA Add option to combine feature domains directly

        bool IsUniversal { get; }
        bool IsEmpty { get; }
        string AttributeName { get; }
        bool ValuesRangeOverlap(ISelector<TValue> other);
        bool Covers(IDataVector<TValue> example);
        ISelector<TValue> Intersect(ISelector<TValue> other);
        bool IsMoreGeneralThan(ISelector<TValue> other);
    }
}