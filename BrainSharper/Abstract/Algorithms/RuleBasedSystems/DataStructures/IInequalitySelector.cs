namespace BrainSharper.Abstract.Algorithms.RuleBasedSystems.DataStructures
{
    public enum InequalityOperation
    {
        LessThan,
        LessThanOrEqual,
        GreaterThan,
        GreaterThanOrEqual
    }

    public interface IInequalitySelector : INumericSelector
    {
        InequalityOperation InequalityOperation { get; }
    }
}
