namespace BrainSharper.Abstract.Algorithms.DecisionTrees.Processors
{
    using System.Collections.Generic;

    public interface INumericalSplitQualityChecker : ISplitQualityChecker
    {
        double CalculateSplitQuality(double initialEntropy, int totalRowsCount, IEnumerable<IList<double>> splittedSubSetsVariances);
    }
}
