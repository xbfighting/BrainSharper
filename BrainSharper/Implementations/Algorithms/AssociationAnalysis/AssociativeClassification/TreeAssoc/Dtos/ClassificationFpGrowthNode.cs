using System.Collections.Generic;
using System.Runtime.InteropServices;
using BrainSharper.Implementations.Algorithms.AssociationAnalysis.DataStructures.FPGrowth;

namespace BrainSharper.Implementations.Algorithms.AssociationAnalysis.AssociativeClassification.TreeAssoc.Dtos
{
    public class ClassificationFpGrowthNode<TValue, TClassLabel> : FpGrowthNode<TValue>
    {
        public ClassificationFpGrowthNode()
        {
        }

        public ClassificationFpGrowthNode(
            TValue value,
            IDictionary<TClassLabel, ClassLabelCountInfo<TClassLabel>> classLabelDistributions,
            bool isLeaf = false, 
            double count = 0, 
            IList<int> transactionIds = null, 
            IList<FpGrowthNode<TValue>> children = null) 
            : base(value, isLeaf, count, transactionIds, children)
        {
            ClassLabelDistributions = classLabelDistributions;
        }

        public ClassificationFpGrowthNode(IDictionary<TClassLabel, ClassLabelCountInfo<TClassLabel>> classLabelDistributions)
        {
            ClassLabelDistributions = classLabelDistributions;
        }

        public IDictionary<TClassLabel, ClassLabelCountInfo<TClassLabel>> ClassLabelDistributions { get; }

        public void AddOrIncrementClassLabelCount(TClassLabel classLabel, int count)
        {
            if (ClassLabelDistributions.ContainsKey(classLabel))
            {
                ClassLabelDistributions[classLabel].IncrementCount(count);
            }
            else
            {
                ClassLabelDistributions.Add(classLabel, new ClassLabelCountInfo<TClassLabel>(classLabel, count));
            }
        }

        public override FpGrowthNode<TValue> CopyNode()
        {
            return new ClassificationFpGrowthNode<TValue, TClassLabel>(
                Value,
                ClassLabelDistributions,
                IsLeaf,
                Count,
                TransactionIds,
                Children)
            {
                Parent = Parent
            };
        }
    }
}
