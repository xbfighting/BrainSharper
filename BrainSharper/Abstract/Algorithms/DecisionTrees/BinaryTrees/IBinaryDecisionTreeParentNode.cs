using System.Collections.Generic;

namespace BrainSharper.Abstract.Algorithms.DecisionTrees.BinaryTrees
{
    public interface IBinaryDecisionTreeParentNode<TDecisionValue> : IDecisionTreeNode
    {
        IDecisionTreeNode LeftChild { get; }
        IBinaryDecisionTreeChildLink LeftChildLink { get; }

        IDecisionTreeNode RightChild { get; }
        IBinaryDecisionTreeChildLink RightChildLink { get; }

        TDecisionValue DecisionValue { get; }
        IDictionary<IBinaryDecisionTreeChildLink, IDecisionTreeNode> TestResultWithChildren { get; }
    }
}
