namespace BrainSharper.Implementations.Algorithms.DecisionTrees.DataStructures
{
    using System.Collections.Generic;

    using BrainSharper.Abstract.Algorithms.DecisionTrees.DataStructures;

    public class RegressionAndModelLeaf : IDecisionTreeRegressionAndModelLeaf
    {
        public RegressionAndModelLeaf(string decisionFeatureName, IList<double> modelWeights, double decisionMeanValue)
        {
            DecisionFeatureName = decisionFeatureName;
            ModelWeights = modelWeights;
            DecisionMeanValue = decisionMeanValue;
        }

        public bool IsLeaf => true;

        public string DecisionFeatureName { get; }

        public object LeafValue => DecisionMeanValue;

        public IList<double> ModelWeights { get; }

        public double DecisionMeanValue { get; }
    }
}
