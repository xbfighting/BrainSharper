using System.Collections.Generic;
using BrainSharper.Abstract.Algorithms.Infrastructure;
using BrainSharper.Abstract.Data;

namespace BrainSharper.Abstract.Algorithms.RuleInduction.DataStructures
{
    public interface IRulesList<TValue> : IPredictionModel
    {
        IList<IRule<TValue>> Rules { get; }
        IDataItem<TValue> Default { get; }
    }
}