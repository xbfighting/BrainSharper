namespace BrainSharper.Abstract.Algorithms.DecisionTrees.Processors
{
    using System.Collections.Generic;

    using Data;

    using DataStructures;
    using DataStructures.BinaryTrees;

    public interface IBinaryDataSplitter : IDataSplitter
    {
        // TODO: consider using ISPlittingResult as a result type
        IList<ISplittedData> SplitData(
            IDataFrame dataToSplit, 
            IBinarySplittingParams splttingParams);
    }
}
