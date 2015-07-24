using System;
using BrainSharper.Abstract.Algorithms.DecisionTrees.DataStructures;
using BrainSharper.Abstract.Algorithms.DecisionTrees.Processors;
using BrainSharper.Abstract.Data;

namespace BrainSharper.Implementations.Algorithms.DecisionTrees.Processors
{
    public class RegressionAndModelDecisionTreeLeafBuilder : ILeafBuilder
    {
        public IDecisionTreeLeaf BuildLeaf(IDataVector<object> finalValues, string dependentFeatureName)
        {
            throw new NotImplementedException();
        }
    }
}
