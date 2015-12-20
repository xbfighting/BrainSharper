﻿using System.Collections.Generic;

namespace BrainSharper.Abstract.Algorithms.DecisionTrees.DataStructures
{
    /// <summary>
    ///     Model tree leaf. It contains the regression model weights used to approximate the target value
    /// </summary>
    public interface IDecisionTreeRegressionAndModelLeaf : IDecisionTreeLeaf
    {
        IList<double> ModelWeights { get; }
        double DecisionMeanValue { get; }
    }
}