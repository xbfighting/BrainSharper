namespace BrainSharper.Abstract.Algorithms.DecisionTrees.Processors
{
    using System.Collections.Generic;

    public interface ICategoricalSplitQualityChecker : ISplitQualityChecker
    {
        double CalculateSplitQuality(double initialEntropy, int totalRowsCount, IList<IList<int>> elementsInGroupsCounts);
    }
}
