using System.Collections.Generic;

namespace BrainSharper.Abstract.Algorithms.DecisionTrees.DataStructures.BinaryTrees
{
    public interface IBinaryDecisionTreeParentNode : IDecisionTreeNode
    {
        IDecisionTreeNode LeftChild { get; }
        IBinaryDecisionTreeLink LeftChildLink { get; }

        IDecisionTreeNode RightChild { get; }
        IBinaryDecisionTreeLink RightChildLink { get; }

        object DecisionValue { get; }
        bool IsValueNumeric { get; }
        IDictionary<IBinaryDecisionTreeLink, IDecisionTreeNode> TestResultsWithChildren { get; }
    }
}
