using System.Collections.Generic;

namespace BrainSharper.Abstract.Algorithms.DecisionTrees.Processors
{
    public interface ICategoricalSplitQualityChecker : ISplitQualityChecker
    {
        double CalculateSplitQuality(double initialEntropy, int totalRowsCount, IList<IList<int>> elementsInGroupsCounts);
    }
}