namespace BrainSharper.Abstract.Algorithms.DecisionTrees.DataStructures
{
    public interface IDecisionTreeLink<TTestResult>
    {
        double InstancesPercentage { get; }

        long InstancesCount { get; }

        TTestResult TestResult { get; }
    }
}
