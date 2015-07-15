using System.Collections.Generic;
using BrainSharper.Abstract.Algorithms.DecisionTrees.DataStructures;
using BrainSharper.Abstract.Algorithms.DecisionTrees.DataStructures.BinaryTrees;
using BrainSharper.Abstract.Data;

namespace BrainSharper.Abstract.Algorithms.DecisionTrees.Processors
{
    public interface IBinaryDataSplitter<T> : IDataSplitter
    {
        IList<ISplittingResult> SplitData(
            IDataFrame dataToSplit, 
            IBinarySplittingParams<T> splttingParams);
    }
}
