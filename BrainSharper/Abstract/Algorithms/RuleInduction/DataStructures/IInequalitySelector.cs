namespace BrainSharper.Abstract.Algorithms.RuleInduction.DataStructures
{
    public enum InequalityOperation
    {
        LessThan,
        LessThanOrEqual,
        GreaterThan,
        GreaterThanOrEqual
    }

    public interface IInequalitySelector<TValue> : INumericSelector<TValue>
    {
        InequalityOperation InequalityOperation { get; }
    }
}
