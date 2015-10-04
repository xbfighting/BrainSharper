namespace BrainSharper.Abstract.Algorithms.RuleInduction.DataStructures
{
    using System.Collections.Generic;

    public interface IDisjunctiveSelector<TValue> : ISelector<TValue>
    {
        ISet<object> AllowedValues { get; }
    }
}
