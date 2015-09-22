namespace BrainSharper.Abstract.Algorithms.RuleBasedSystems.DataStructures
{
    using System.Collections.Generic;

    public interface IDisjunctiveSelector : ISelector
    {
        ISet<object> AllowedValues { get; }
    }
}
