namespace BrainSharper.Implementations.Algorithms.RuleInduction.DataStructures
{
    using System.Collections.Generic;
    using System.Linq;

    using BrainSharper.Abstract.Algorithms.RuleInduction.DataStructures;
    using BrainSharper.Abstract.Data;
    using BrainSharper.General.Utils;

    public class Rule<TValue> : IRule<TValue>
    {
        public Rule(IList<IComplex<TValue>> antecedents, IDataItem<TValue> consequent, LogicOperator logicOperator = LogicOperator.And)
        {
            this.AntecedentLogicOperator = logicOperator;
            this.Antecedents = antecedents;
            this.Consequent = consequent;
        }

        public LogicOperator AntecedentLogicOperator { get; }

        public IList<IComplex<TValue>> Antecedents { get; }

        public IDataItem<TValue> Consequent { get; }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj))
            {
                return false;
            }
            if (ReferenceEquals(this, obj))
            {
                return true;
            }
            if (obj.GetType() != this.GetType())
            {
                return false;
            }
            return this.Equals((Rule<TValue>)obj);
        }

        public override int GetHashCode()
        {
            unchecked
            {
                var hashCode = (int)this.AntecedentLogicOperator;
                hashCode = (hashCode * 397) ^ (this.Antecedents?.Sum(ant => ant.GetHashCode()) ?? 0);
                hashCode = (hashCode * 397) ^ (this.Consequent != null ? this.Consequent.GetHashCode() : 0);
                return hashCode;
            }
        }

        protected bool Equals(Rule<TValue> other)
        {
            return this.AntecedentLogicOperator == other.AntecedentLogicOperator && this.Antecedents.IsEquivalentTo(other.Antecedents) && Equals(this.Consequent, other.Consequent);
        }
    }
}
