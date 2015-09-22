namespace BrainSharper.Abstract.Algorithms.RuleBasedSystems.DataStructures
{
    using BrainSharper.Abstract.Data;

    public interface ISelector
    {
        bool IsUniversal { get; }
        bool IsEmpty { get; }
        string AttributeName { get; }

        bool Covers<TValue>(IDataVector<TValue> example);
        ISelector Intersect(ISelector other);
        bool IsMoreDetailedThan(ISelector other);
    }
}
