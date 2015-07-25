using System.Collections.Generic;
using BrainSharper.Abstract.Algorithms.DecisionTrees.DataStructures;
using BrainSharper.Abstract.Data;

namespace BrainSharper.Abstract.Algorithms.DecisionTrees.Processors
{
    public interface ISplitQualityChecker<TTestResult>
    {
        //TODO: replace IList<ISplittedData> with ISplitResult!!!
        double GetInitialEntropy(IDataFrame baseData, string dependentFeatureName);
        double CalculateSplitQuality(IDataFrame baseData, IList<ISplittedData<TTestResult>> splittingResults, string dependentFeatureName);
        double CalculateSplitQuality(double initialEntropy, int totalRowsCount, IList<ISplittedData<TTestResult>> splittingResults, string dependentFeatureName);
    }
}
