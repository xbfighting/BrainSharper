using System.Collections.Generic;
using System.Linq;
using BrainSharper.Abstract.Algorithms.DecisionTrees.DataStructures;

namespace BrainSharper.Implementations.Algorithms.DecisionTrees.DataStructures
{
    public class DecisionTreeParentNode : IDecisionTreeParentNode
    {
        public DecisionTreeParentNode(
            bool isLeaf, 
            string decisionFeatureName, 
            IDictionary<IDecisionTreeLink, IDecisionTreeNode> linksToChildren)
        {
            DecisionFeatureName = decisionFeatureName;
            Children = linksToChildren?.Values.ToList() ?? new List<IDecisionTreeNode>();
            LinksToChildren = linksToChildren ?? new Dictionary<IDecisionTreeLink, IDecisionTreeNode>();
        }

        public bool IsLeaf => false;
        public string DecisionFeatureName { get; }
        public IList<IDecisionTreeNode> Children { get; }
        public IDictionary<IDecisionTreeLink, IDecisionTreeNode> LinksToChildren { get; }
    }
}
