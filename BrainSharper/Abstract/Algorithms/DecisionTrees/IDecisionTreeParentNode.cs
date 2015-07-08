using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BrainSharper.Abstract.Algorithms.DecisionTrees
{
    public interface IDecisionTreeParentNode : IDecisionTreeNode
    {
        IList<IDecisionTreeNode> Children { get; }
        IDictionary<IDecisionTreeLink, IDecisionTreeNode> LinksToChildren { get; }
    }
}
