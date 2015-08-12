namespace BrainSharper.Abstract.Algorithms.DecisionTrees.Processors
{
    using System.Collections.Generic;

    using Data;

    using DataStructures;

    public interface ISplitQualityChecker
    {
        //TODO: replace IList<ISplittedData> with ISplitResult!!!
        double GetInitialEntropy(IDataFrame baseData, string dependentFeatureName);
        double CalculateSplitQuality(IDataFrame baseData, IList<ISplittedData> splittingResults, string dependentFeatureName);
        double CalculateSplitQuality(double initialEntropy, int totalRowsCount, IList<ISplittedData> splittingResults, string dependentFeatureName);
        double CalculateSplitQuality(double initialEntropy, int totalRowsCount, IList<IList<int>> elementsInGroupsCounts);
    }
}
