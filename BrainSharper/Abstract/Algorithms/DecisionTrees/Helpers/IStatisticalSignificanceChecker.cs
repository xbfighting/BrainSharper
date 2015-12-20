using BrainSharper.Abstract.Algorithms.DecisionTrees.DataStructures;
using BrainSharper.Abstract.Data;

namespace BrainSharper.Abstract.Algorithms.DecisionTrees.Helpers
{
    public interface IStatisticalSignificanceChecker
    {
        bool IsSplitStatisticallySignificant(
            IDataFrame initialDataFrame,
            ISplittingResult splittingResults,
            string dependentFeatureName);
    }
}