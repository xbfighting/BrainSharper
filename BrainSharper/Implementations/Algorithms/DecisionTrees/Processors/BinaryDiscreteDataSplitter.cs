using System;
using System.Collections.Generic;
using System.Data;
using BrainSharper.Abstract.Algorithms.DecisionTrees.DataStructures;
using BrainSharper.Abstract.Algorithms.DecisionTrees.DataStructures.BinaryTrees;
using BrainSharper.Abstract.Algorithms.DecisionTrees.Processors;
using BrainSharper.Abstract.Algorithms.Infrastructure;
using BrainSharper.Abstract.Data;
using BrainSharper.General.Utils;
using BrainSharper.Implementations.Algorithms.DecisionTrees.DataStructures;
using BrainSharper.Implementations.Algorithms.DecisionTrees.DataStructures.BinaryDecisionTrees;

namespace BrainSharper.Implementations.Algorithms.DecisionTrees.Processors
{
    public class BinaryDiscreteDataSplitter<T> : IBinaryDataSplitter<T>
    {
        public IList<ISplittingResult> SplitData(IDataFrame dataToSplit, IBinarySplittingParams<T> splttingParams)
        {
            var splittingFeatureName = splttingParams.SplitOnFeature;
            var splittingFeatureValue = splttingParams.SplitOnValue;

            Predicate<DataRow> rowFilter =
                row => Convert.ChangeType(row[splittingFeatureName], typeof(T))
                            .Equals(splittingFeatureValue);

            var filteringResult = dataToSplit.GetRowsIndicesWhere(rowFilter);
            var rowsMeetingCriteria = filteringResult.IndicesOfRowsMeetingCriteria;
            var rowsNotMeetingCriteria = filteringResult.IndicesOfRowsNotMeetingCriteria;

            var positiveDataFrame = dataToSplit.GetSubsetByRows(rowsMeetingCriteria);
            var negativeDataFrame = dataToSplit.GetSubsetByRows(rowsNotMeetingCriteria);

            var totalRowsCount = (double)dataToSplit.RowCount;
            var splitResults = new List<ISplittingResult>();
            foreach (var subset in new[] { positiveDataFrame, negativeDataFrame })
            {
                var splitLink = GetSubsetStatistic(subset, totalRowsCount);
                splitResults.Add(new SplittingResult(splitLink, subset));
            }
            return splitResults;
        }

        public IList<ISplittingResult> SplitData(
            IDataFrame dataToSplit,
            ISplittingParams splttingParams)
        {
            if (!(splttingParams is IBinarySplittingParams<T>))
            {
                throw new ArgumentException("Invalid splitting params passed to binary splitter");
            }
            return SplitData(dataToSplit, (IBinarySplittingParams<T>) splttingParams);
        }

        private static BinaryDecisionTreeLink GetSubsetStatistic(IDataFrame subset, double totalRowsCount)
        {
            return new BinaryDecisionTreeLink(
                subset.RowCount/totalRowsCount,
                subset.RowCount, true);
        }
    }
}
