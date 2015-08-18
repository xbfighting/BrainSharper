namespace BrainSharper.Abstract.Algorithms.DecisionTrees.Processors
{
    using Data;

    using DataStructures;

    /// <summary>
    /// Selects the winning value in the given decision tree leaf - no matter if it is discrette or numerical
    /// </summary>
    public interface ILeafBuilder
    {
        IDecisionTreeLeaf BuildLeaf(IDataFrame finalData, string dependentFeatureName);
    }
}
