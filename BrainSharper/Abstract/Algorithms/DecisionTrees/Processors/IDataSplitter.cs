namespace BrainSharper.Abstract.Algorithms.DecisionTrees.Processors
{
    using System.Collections.Generic;

    using DataStructures;
    using Data;

    public interface IDataSplitter<TTestResult>
    {
        IList<ISplittedData> SplitData(
            IDataFrame dataToSplit, 
            ISplittingParams splttingParams);
    }
}
