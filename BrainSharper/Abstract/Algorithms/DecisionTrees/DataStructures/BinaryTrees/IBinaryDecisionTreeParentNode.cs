using System.Collections.Generic;

namespace BrainSharper.Abstract.Algorithms.DecisionTrees.DataStructures.BinaryTrees
{
    public interface IBinaryDecisionTreeParentNode<TDecisionValue> : IDecisionTreeNode
    {
        IDecisionTreeNode LeftChild { get; }
        IBinaryDecisionTreeLink LeftChildLink { get; }

        IDecisionTreeNode RightChild { get; }
        IBinaryDecisionTreeLink RightChildLink { get; }

        TDecisionValue DecisionValue { get; }
        IDictionary<IBinaryDecisionTreeLink, IDecisionTreeNode> TestResultsWithChildren { get; }
    }
}
