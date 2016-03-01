using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;

namespace BrainSharper.Implementations.Algorithms.AssociationAnalysis.DataStructures.FPGrowth
{
    public class FpGrowthNode<TValue>
    {
        public FpGrowthNode(
            TValue value,
            bool isLeaf = false,
            double count = 0.0,
            IList<int> transactionIds = null, //TODO: AAAA Fpgrowth, think if this is needed in constructor
            IList<FpGrowthNode<TValue>> children = null)
        {
            Value = value;
            IsLeaf = isLeaf;
            Children = children ?? new List<FpGrowthNode<TValue>>();
            TransactionIds = transactionIds ?? new List<int>();
            Count = count;
            IsRoot = false;
        }

        public FpGrowthNode()
            : this(default(TValue))
        {
            IsRoot = true;
        }

        public bool IsRoot { get; }

        public bool IsLeaf { get; set; }
        public TValue Value { get; }

        public bool HasParent => Parent != null;
        public FpGrowthNode<TValue> Parent { get; set; }

        public bool HasChildren => Children != null && Children.Any();
        public bool HasBranches => HasChildren && Children.Count > 1;
        public IList<FpGrowthNode<TValue>> Children { get; set; }

        public IList<int> TransactionIds { get; }
        public double Count { get; set; }

        public void IncrementCountBy(double count)
        {
            Count += count;
        }

        public void AddChild(FpGrowthNode<TValue> child)
        {
            Children.Add(child);
            child.Parent = this;
            if (IsLeaf)
            {
                IsLeaf = false;
            }
        }

        public string PrintStructure(int indent = 0)
        {
            var sb = new StringBuilder();
            var parentIndent = new string(' ', indent);
            var parentSpecification = $"{parentIndent}{Value}: {Count}";
            var childrenIndent = indent + "  ";
            sb.Append(parentSpecification);
            foreach (var child in Children)
            {
                sb.Append($"\n {parentIndent} | {child.PrintStructure(indent + 1)}");
            }
            return sb.ToString();
        }

        public FpGrowthNode<TValue> CopyNode()
        {
            return new FpGrowthNode<TValue>(
                Value,
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
