using System.Collections.Generic;

namespace BrainSharper.Abstract.Algorithms.DecisionTrees.Processors
{
    public interface INumericalSplitQualityChecker : ISplitQualityChecker
    {
        double CalculateSplitQuality(double initialEntropy, int totalRowsCount,
            IEnumerable<IList<double>> splittedSubSetsVariances);
    }
}