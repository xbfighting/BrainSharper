using System.Collections.Generic;
using BrainSharper.Abstract.Algorithms.DecisionTrees.DataStructures;
using BrainSharper.Abstract.Data;

namespace BrainSharper.Abstract.Algorithms.DecisionTrees.Processors
{
    public interface IDataSplitter<TTestResult>
    {
        IList<ISplittedData<TTestResult>> SplitData(
            IDataFrame dataToSplit, 
            ISplittingParams splttingParams);
    }
}
