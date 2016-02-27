using System.Collections.Generic;

namespace BrainSharper.Implementations.Algorithms.AssociationAnalysis.DataStructures.FPGrowth
{
    public class FpGrowthHeaderTable<TValue>
    {
        public FpGrowthHeaderTable(IDictionary<TValue, IList<FpGrowthNode<TValue>>> valuesToNodesMapping)
        {
            ValuesToNodesMapping = valuesToNodesMapping;
        }

        public IDictionary<TValue, IList<FpGrowthNode<TValue>>> ValuesToNodesMapping { get; }

        public void AddNodeToMapping(FpGrowthNode<TValue> nodeToAdd)
        {
            if (ValuesToNodesMapping.ContainsKey(nodeToAdd.Value))
            {
                ValuesToNodesMapping[nodeToAdd.Value].Add(nodeToAdd);
            }
            else
            {
                ValuesToNodesMapping.Add(nodeToAdd.Value, new List<FpGrowthNode<TValue>> { nodeToAdd });
            }
        }

        public IList<FpGrowthNode<TValue>> GetNodesForValue(TValue value)
        {
            return ValuesToNodesMapping.ContainsKey(value)
                ? ValuesToNodesMapping[value]
                : new FpGrowthNode<TValue>[0];
        }

    }
}
