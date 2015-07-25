using System.Collections.Generic;

namespace BrainSharper.Abstract.Algorithms.DecisionTrees.DataStructures.BinaryTrees
{
    public interface IBinaryDecisionTreeParentNode : IDecisionTreeNode
    {
        /// <summary>
        /// Always represents the child, for which test evaluated to FALSE
        /// </summary>
        IDecisionTreeNode LeftChild { get; }
        IBinaryDecisionTreeLink LeftChildLink { get; }

        /// <summary>
        /// Always represents the child, for which test evaluated to TRUE
        /// </summary>
        IDecisionTreeNode RightChild { get; }
        IBinaryDecisionTreeLink RightChildLink { get; }

        object DecisionValue { get; }
        bool IsValueNumeric { get; }
    }
}
