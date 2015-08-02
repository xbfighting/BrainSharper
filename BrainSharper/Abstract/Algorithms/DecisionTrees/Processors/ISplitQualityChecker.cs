namespace BrainSharper.Abstract.Algorithms.DecisionTrees.Processors
{
    using System.Collections.Generic;
    using DataStructures;
    using Data;

    public interface ISplitQualityChecker
    {
        //TODO: replace IList<ISplittedData> with ISplitResult!!!
        double GetInitialEntropy(IDataFrame baseData, string dependentFeatureName);
        double CalculateSplitQuality(IDataFrame baseData, IList<ISplittedData> splittingResults, string dependentFeatureName);
        double CalculateSplitQuality(double initialEntropy, int totalRowsCount, IList<ISplittedData> splittingResults, string dependentFeatureName);
    }
}
