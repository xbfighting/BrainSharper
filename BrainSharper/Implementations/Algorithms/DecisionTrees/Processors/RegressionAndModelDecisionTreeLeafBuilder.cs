using System;
using BrainSharper.Abstract.Algorithms.DecisionTrees.DataStructures;
using BrainSharper.Abstract.Algorithms.DecisionTrees.Processors;
using BrainSharper.Abstract.Data;

namespace BrainSharper.Implementations.Algorithms.DecisionTrees.Processors
{
    public class RegressionAndModelDecisionTreeLeafBuilder : ILeafBuilder<double>
    {
        public IDecisionTreeLeaf<double> BuildLeaf(IDataVector<double> finalValues, string dependentFeatureName)
        {
            //TODO: think about refactoring the method signatre to include WHOLE DataFrame
            throw new NotImplementedException();
        }
    }
}
