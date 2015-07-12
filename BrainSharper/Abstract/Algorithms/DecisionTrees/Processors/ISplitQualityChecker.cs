using System.Collections.Generic;
using BrainSharper.Abstract.Algorithms.DecisionTrees.DataStructures;

namespace BrainSharper.Abstract.Algorithms.DecisionTrees.Processors
{
    public interface ISplitQualityChecker
    {
        double CalculateSplitQuality(IList<IDecisionTreeLink> decisionTreeLinks);
    }
}
