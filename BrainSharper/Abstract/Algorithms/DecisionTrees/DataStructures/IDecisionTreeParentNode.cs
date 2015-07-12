using System.Collections.Generic;

namespace BrainSharper.Abstract.Algorithms.DecisionTrees.DataStructures
{
    public interface IDecisionTreeParentNode : IDecisionTreeNode
    {
        IList<IDecisionTreeNode> Children { get; }
        IDictionary<IDecisionTreeLink, IDecisionTreeNode> LinksToChildren { get; }
    }
}
