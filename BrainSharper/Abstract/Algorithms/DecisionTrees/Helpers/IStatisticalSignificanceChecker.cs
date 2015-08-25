namespace BrainSharper.Abstract.Algorithms.DecisionTrees.Helpers
{
    using Data;
    using DataStructures;

    public interface IStatisticalSignificanceChecker
    {
        bool IsSplitStatisticallySignificant(
            IDataFrame initialDataFrame, 
            ISplittingResult splittingResults, 
            string dependentFeatureName);
    }
}
