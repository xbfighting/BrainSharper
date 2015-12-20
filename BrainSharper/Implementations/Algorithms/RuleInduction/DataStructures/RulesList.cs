using System.Collections.Generic;
using BrainSharper.Abstract.Algorithms.RuleInduction.DataStructures;
using BrainSharper.Abstract.Data;

namespace BrainSharper.Implementations.Algorithms.RuleInduction.DataStructures
{
    public class RulesList<TValue> : IRulesList<TValue>
    {
        public RulesList(IList<IRule<TValue>> rules, IDataItem<TValue> defaultVal)
        {
            Rules = rules;
            Default = defaultVal;
        }

        public IList<IRule<TValue>> Rules { get; }
        public IDataItem<TValue> Default { get; }
    }
}