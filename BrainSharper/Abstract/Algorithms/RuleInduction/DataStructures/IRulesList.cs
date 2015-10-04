namespace BrainSharper.Abstract.Algorithms.RuleInduction.DataStructures
{
    using System.Collections.Generic;
    using Infrastructure;

    using Data;

    public interface IRulesList<TValue> : IPredictionModel
    {
        IList<IRule<TValue>> Rules { get; }
        IDataItem<TValue> Default { get; }
    }
}
