using System.Collections.Generic;

namespace BrainSharper.Abstract.Algorithms.RuleInduction.DataStructures
{
    public interface IDisjunctiveSelector<TValue> : ISelector<TValue>
    {
        ISet<object> AllowedValues { get; }
    }
}