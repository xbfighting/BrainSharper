namespace BrainSharper.Abstract.Algorithms.DecisionTrees.DataStructures
{
    public interface IDecisionTreeLink
    {
        double InstancesPercentage { get; }
        long InstancesCount { get; }
        object TestResult { get; }
    }
}
