using BrainSharper.Abstract.Algorithms.DecisionTrees.DataStructures;
using BrainSharper.Abstract.Data;

namespace BrainSharper.Abstract.Algorithms.DecisionTrees.Processors
{
    /// <summary>
    /// Selects the winning value in the given decision tree leaf - no matter if it is discrette or numerical
    /// </summary>
    public interface ILeafBuilder<T>
    {
        IDecisionTreeLeaf<T> BuildLeaf(IDataVector<T> finalValues, string dependentFeatureName);
    }
}
