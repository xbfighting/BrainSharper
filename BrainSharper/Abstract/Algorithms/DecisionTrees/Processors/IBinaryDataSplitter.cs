using System.Collections.Generic;
using BrainSharper.Abstract.Algorithms.DecisionTrees.DataStructures;
using BrainSharper.Abstract.Algorithms.DecisionTrees.DataStructures.BinaryTrees;
using BrainSharper.Abstract.Data;

namespace BrainSharper.Abstract.Algorithms.DecisionTrees.Processors
{
    public interface IBinaryDataSplitter<TDecisionValueType> : IDataSplitter<bool>
    {
        //TODO: consider using ISPlittingResult as a result type
        IList<ISplittedData<bool>> SplitData(
            IDataFrame dataToSplit, 
            IBinarySplittingParams<TDecisionValueType> splttingParams);
    }
}
