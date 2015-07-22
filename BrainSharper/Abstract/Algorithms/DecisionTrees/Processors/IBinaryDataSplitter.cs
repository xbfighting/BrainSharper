using System.Collections.Generic;
using BrainSharper.Abstract.Algorithms.DecisionTrees.DataStructures;
using BrainSharper.Abstract.Algorithms.DecisionTrees.DataStructures.BinaryTrees;
using BrainSharper.Abstract.Data;

namespace BrainSharper.Abstract.Algorithms.DecisionTrees.Processors
{
    public interface IBinaryDataSplitter<T> : IDataSplitter
    {
        //TODO: consider using ISPlittingResult as a result type
        IList<ISplittedData> SplitData(
            IDataFrame dataToSplit, 
            IBinarySplittingParams<T> splttingParams);
    }
}
