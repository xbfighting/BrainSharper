namespace BrainSharper.Abstract.Algorithms.RuleBasedSystems.DataStructures
{
    public interface IRangeSelector : INumericSelector
    {
        double RangeFrom { get; }
        bool FromInclusive { get; }

        double RangeTo { get; }
        bool ToInclusive { get; }
    }
}
