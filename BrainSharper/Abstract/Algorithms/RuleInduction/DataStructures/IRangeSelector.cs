namespace BrainSharper.Abstract.Algorithms.RuleInduction.DataStructures
{
    public interface IRangeSelector<TValue> : INumericSelector<TValue>
    {
        double RangeFrom { get; }
        bool FromInclusive { get; }

        double RangeTo { get; }
        bool ToInclusive { get; }
    }
}
