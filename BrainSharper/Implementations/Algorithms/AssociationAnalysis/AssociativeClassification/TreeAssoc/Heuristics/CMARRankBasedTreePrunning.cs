using System.Collections.Generic;
using BrainSharper.Abstract.Algorithms.AssociationAnalysis.AssociativeClassification;
using BrainSharper.Abstract.Data;
using BrainSharper.Implementations.Algorithms.AssociationAnalysis.DataStructures.FPGrowth;

namespace BrainSharper.Implementations.Algorithms.AssociationAnalysis.AssociativeClassification.TreeAssoc.Heuristics
{
    public class CMARRankBasedTreePrunning<TValue>
    {
        public bool CanRuleBeInserted(
            IClassificationTransaction<TValue> transcationInQuestion,
            FpGrowthNode<IDataItem<TValue>> tree)
        {
            return false;
        }

        protected bool TraverseTree(
            IClassificationTransaction<TValue> transcationInQuestion,
            ISet<string> alreadyProcessedAttributes,
            FpGrowthNode<IDataItem<TValue>> tree
            )
        {
            return false;
        }
    }
}
