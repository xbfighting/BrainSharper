using System.Collections.Generic;
using System.Linq;
using BrainSharper.Abstract.Algorithms.DecisionTrees.DataStructures;
using BrainSharper.Abstract.Algorithms.DecisionTrees.Processors;
using BrainSharper.Abstract.Data;
using BrainSharper.Implementations.Algorithms.DecisionTrees.DataStructures;

namespace BrainSharper.Implementations.Algorithms.DecisionTrees.Processors
{
    public class MultiValueDiscreteDataSplitter : IDataSplitter
    {
        public IList<ISplittedData> SplitData(IDataFrame dataToSplit, ISplittingParams splttingParams)
        {
            var splitFeature = splttingParams.SplitOnFeature;
            var totalRowsCount = dataToSplit.RowCount;
            var uniqueValues = dataToSplit.GetColumnVector(splitFeature).Distinct();
            var splittedData = new List<ISplittedData>();
            //TODO: AAA emarassingly parallel - test it for performance
            foreach (var uniqueValue in uniqueValues)
            {
                var query = BuildQuery(splitFeature, uniqueValue);
                var splitResult = dataToSplit.GetSubsetByQuery(query);
                var subsetCount = splitResult.RowCount;
                var link = new DecisionLink(
                    CalcInstancesPercentage(totalRowsCount, subsetCount),
                    subsetCount,
                    uniqueValue);
                splittedData.Add(new SplittedData(link, splitResult));
            }
            return splittedData;
        }

        private string BuildQuery(string featureName, object featureValue)
        {
            return $"[{featureName}] = '{featureValue}'";
        }

        private double CalcInstancesPercentage(int totalRowsCount, int splitRowsCount)
        {
            return splitRowsCount/(double) totalRowsCount;
        }
    }
}