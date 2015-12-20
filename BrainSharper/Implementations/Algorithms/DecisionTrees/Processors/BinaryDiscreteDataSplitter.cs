using System;
using System.Collections.Generic;
using System.Data;
using BrainSharper.Abstract.Algorithms.DecisionTrees.DataStructures;
using BrainSharper.Abstract.Algorithms.DecisionTrees.DataStructures.BinaryTrees;
using BrainSharper.Abstract.Algorithms.DecisionTrees.Processors;
using BrainSharper.Abstract.Data;
using BrainSharper.Implementations.Algorithms.DecisionTrees.DataStructures;
using BrainSharper.Implementations.Algorithms.DecisionTrees.DataStructures.BinaryDecisionTrees;

namespace BrainSharper.Implementations.Algorithms.DecisionTrees.Processors
{
    public class BinaryDiscreteDataSplitter : IBinaryDataSplitter
    {
        public IList<ISplittedData> SplitData(IDataFrame dataToSplit, IBinarySplittingParams splttingParams)
        {
            var queries = BuildQueries(splttingParams.SplitOnFeature, splttingParams.SplitOnValue);
            var splitResults = new List<ISplittedData>();
            var totalRowsCount = (double) dataToSplit.RowCount;

            foreach (var boolAndQuery in queries)
            {
                var resultDataFrame = dataToSplit.GetSubsetByQuery(boolAndQuery.Value);
                splitResults.Add(new SplittedData(GetSubsetLink(resultDataFrame, totalRowsCount, boolAndQuery.Key),
                    resultDataFrame));
            }
            return splitResults;
        }

        public IList<ISplittedData> SplitData(
            IDataFrame dataToSplit,
            ISplittingParams splttingParams)
        {
            if (!(splttingParams is IBinarySplittingParams))
            {
                throw new ArgumentException("Invalid splitting params passed to binary splitter");
            }
            return SplitData(dataToSplit, (IBinarySplittingParams) splttingParams);
        }

        protected virtual Predicate<DataRow> BuildSplittingFunction(string splittingFeatureName,
            object splittingFeatureValue)
        {
            Predicate<DataRow> rowFilter = row => row[splittingFeatureName].Equals(splittingFeatureValue);
            return rowFilter;
        }

        protected virtual Dictionary<bool, string> BuildQueries(
            string splittingFeatureName,
            object splittingFeatureValue)
        {
            return new Dictionary<bool, string>
            {
                [true] = $"[{splittingFeatureName}] = '{splittingFeatureValue}'",
                [false] = $"[{splittingFeatureName}] <> '{splittingFeatureValue}'"
            };
        }

        private static BinaryDecisionTreeLink GetSubsetLink(IDataFrame subset, double totalRowsCount, bool testResult)
        {
            return new BinaryDecisionTreeLink(
                subset.Any ? subset.RowCount/totalRowsCount : 0,
                subset.RowCount,
                testResult);
        }
    }
}