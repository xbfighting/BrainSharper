namespace BrainSharper.Abstract.Algorithms.DecisionTrees.DataStructures.BinaryTrees
{
    public interface IBinaryRegressionLink : IBinaryDecisionTreeLink
    {
        double Variance { get; }
        double Mean { get; }
    }
}