namespace BrainSharper.Abstract.Algorithms.DecisionTrees
{
    public interface IDecisionTreeLink
    {
        double InstancesPercentage { get; }
        long InstancesCount { get; }
    }
}
