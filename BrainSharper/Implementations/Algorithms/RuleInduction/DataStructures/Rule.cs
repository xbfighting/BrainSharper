using System.Collections.Generic;
using System.Linq;
using BrainSharper.Abstract.Algorithms.RuleInduction.DataStructures;
using BrainSharper.Abstract.Data;
using BrainSharper.General.Utils;

namespace BrainSharper.Implementations.Algorithms.RuleInduction.DataStructures
{
    public class Rule<TValue> : IRule<TValue>
    {
        public Rule(IList<IComplex<TValue>> antecedents, IDataItem<TValue> consequent,
            LogicOperator logicOperator = LogicOperator.And)
        {
            AntecedentLogicOperator = logicOperator;
            Antecedents = antecedents;
            Consequent = consequent;
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
            if (obj.GetType() != GetType())
            {
                return false;
            }
            return Equals((Rule<TValue>) obj);
        }

        public override int GetHashCode()
        {
            unchecked
            {
                var hashCode = (int) AntecedentLogicOperator;
                hashCode = (hashCode*397) ^ (Antecedents?.Sum(ant => ant.GetHashCode()) ?? 0);
                hashCode = (hashCode*397) ^ (Consequent != null ? Consequent.GetHashCode() : 0);
                return hashCode;
            }
        }

        protected bool Equals(Rule<TValue> other)
        {
            return AntecedentLogicOperator == other.AntecedentLogicOperator &&
                   Antecedents.IsEquivalentTo(other.Antecedents) && Equals(Consequent, other.Consequent);
        }
    }
}