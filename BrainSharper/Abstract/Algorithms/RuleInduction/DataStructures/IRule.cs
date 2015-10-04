namespace BrainSharper.Abstract.Algorithms.RuleInduction.DataStructures
{
    using System.Collections.Generic;

    using BrainSharper.Abstract.Data;

    public enum LogicOperator
    {
        And,
        Or
    }

    public interface IRule<TValue>
    {
        LogicOperator AntecedentLogicOperator { get; }
        IList<IComplex<TValue>> Antecedents { get; }
        IDataItem<TValue> Consequent { get; } 
    }
}