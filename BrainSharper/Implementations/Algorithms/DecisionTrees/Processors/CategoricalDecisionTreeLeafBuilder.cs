using System.Collections.Generic;
using System.Linq;
using BrainSharper.Abstract.Algorithms.DecisionTrees.DataStructures;
using BrainSharper.Abstract.Algorithms.DecisionTrees.Processors;
using BrainSharper.Abstract.Data;
using BrainSharper.Implementations.Algorithms.DecisionTrees.DataStructures;

namespace BrainSharper.Implementations.Algorithms.DecisionTrees.Processors
{
    public class CategoricalDecisionTreeLeafBuilder : ILeafBuilder
    {
        public IDecisionTreeLeaf BuildLeaf(IDataFrame finalData, string dependentFeatureName)
        {
            var counts = new Dictionary<object, int>();
            var finalValues = finalData.GetColumnVector(dependentFeatureName);
            foreach (var val in finalValues)
            {
                if (!counts.ContainsKey(val))
                {
                    counts.Add(val, 0);
                }
                counts[val] += 1;
            }
            return new DecisionTreeLeaf(dependentFeatureName, counts.OrderBy(kvp => kvp.Value).Reverse().First().Key);
        }
    }
}