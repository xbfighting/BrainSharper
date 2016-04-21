using System.Collections.Generic;
using System.Linq;
using System.Text;
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

        public IDictionary<TClassLabel, ClassLabelCountInfo<TClassLabel>> ClassLabelDistributions { get; set; }

        public static ClassificationFpGrowthNode<TValue, TClassLabel> FromOther(ClassificationFpGrowthNode<TValue, TClassLabel> other)
        {
            return new ClassificationFpGrowthNode<TValue, TClassLabel>(
                other.Value,
                new Dictionary<TClassLabel, ClassLabelCountInfo<TClassLabel>>(other.ClassLabelDistributions),
                other.IsLeaf,
                other.Count,
                other.TransactionIds,
                other.Children
                );
        }

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

        public override FpGrowthNode<TValue> CopyNode(bool includeChildren = true)
        {
            return new ClassificationFpGrowthNode<TValue, TClassLabel>(
                Value,
                new Dictionary<TClassLabel, ClassLabelCountInfo<TClassLabel>>(ClassLabelDistributions),
                IsLeaf,
                Count,
                TransactionIds,
                includeChildren ? Children : null)
            {
                Parent = Parent
            };
        }

        public override string PrintStructure(int indent = 0)
        {
            var classLabelDistributionsRepr = string.Join(",", ClassLabelDistributions?.Select(kvp => $"{kvp.Key} => {kvp.Value.Count}") ?? new string[0]);
            var sb = new StringBuilder();
            var parentIndent = new string(' ', indent);
            var parentSpecification = $"{parentIndent}{Value} [{Count}]; {classLabelDistributionsRepr}";
            sb.Append(parentSpecification);
            foreach (var child in Children)
            {
                sb.Append($"\n {parentIndent} | {child.PrintStructure(indent + 1)}");
            }
            return sb.ToString();
        }

        public override string ToString()
        {
            var classLabelDistributionsRepr = string.Join(",",
                ClassLabelDistributions.Select(kvp => $"{kvp.Key} => {kvp.Value.Count}"));
            return $"{Value} [{Count}]; {classLabelDistributionsRepr})";
        }
    }
}
