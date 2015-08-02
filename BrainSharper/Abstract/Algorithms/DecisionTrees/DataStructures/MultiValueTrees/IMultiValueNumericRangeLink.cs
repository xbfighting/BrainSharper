namespace BrainSharper.Abstract.Algorithms.DecisionTrees.DataStructures.MultiValueTrees
{
    public interface IMultiValueNumericRangeLink : IDecisionTreeLink
    {
        double RangeStart { get; }
        double RangeEnd { get; }
    }
}
