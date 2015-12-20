namespace BrainSharper.Abstract.FeaturesEngineering
{
    public interface IRange
    {
        string AttributeName { get; }
        bool IsUniversal { get; }
        double RangeFrom { get; }
        double RangeTo { get; }
    }
}