using System.Collections.Generic;
using BrainSharper.Abstract.Algorithms.DecisionTrees.DataStructures;
using BrainSharper.Abstract.Data;

namespace BrainSharper.Abstract.Algorithms.DecisionTrees.Processors
{
    public interface IDataSplitter
    {
        IList<ISplittedData> SplitData(
            IDataFrame dataToSplit,
            ISplittingParams splttingParams);
    }
}