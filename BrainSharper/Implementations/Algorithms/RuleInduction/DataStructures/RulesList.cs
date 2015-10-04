namespace BrainSharper.Implementations.Algorithms.RuleInduction.DataStructures
{
    using System.Collections.Generic;

    using Abstract.Algorithms.RuleInduction.DataStructures;
    using Abstract.Data;

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
