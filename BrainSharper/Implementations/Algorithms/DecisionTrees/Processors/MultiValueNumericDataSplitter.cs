namespace BrainSharper.Implementations.Algorithms.DecisionTrees.Processors
{
    using System.Collections.Concurrent;
    using System.Collections.Generic;
    using System.Linq;

    using Abstract.Algorithms.DecisionTrees.DataStructures;
    using Abstract.Algorithms.DecisionTrees.Processors;
    using Abstract.Data;

    using BrainSharper.Abstract.Algorithms.DecisionTrees.DataStructures.MultiValueTrees;
    using BrainSharper.Implementations.Algorithms.DecisionTrees.DataStructures;
    using BrainSharper.Implementations.Algorithms.DecisionTrees.DataStructures.MultiValueTrees;

    public class MultiValueNumericDataSplitter : IMultiValueNumericDataSplitter
    {
        public IList<ISplittedData> SplitData(IDataFrame dataToSplit, ISplittingParams splttingParams)
        {
            var splitOnFeature = splttingParams.SplitOnFeature;
            var orderedFeatureValues = dataToSplit.GetNumericColumnVector(splitOnFeature)
                .Select((val, index) => new { Value = val, RowIndex = index }).OrderBy(val => val.Value);
            var totalRowsCount = (double)dataToSplit.RowCount;
            var numericRangeSplits = new List<ISplittedData>();

            var firstElement = orderedFeatureValues.First();
            var previousFeatureValue = firstElement.Value;
            var currentFeatureValue = previousFeatureValue;
            var currentClass = dataToSplit[firstElement.RowIndex, splttingParams.DependentFeatureName];
            var previousClass = currentClass;
            var gatheredRows = new List<int>();
            var rangeStart = double.NegativeInfinity;
            var rangeEnd = double.PositiveInfinity;
            foreach (var value in orderedFeatureValues)
            {
                currentFeatureValue = value.Value;
                currentClass = dataToSplit[value.RowIndex, splttingParams.DependentFeatureName];

                if (!Equals(currentClass, previousClass))
                {
                    var halfWayValue = (previousFeatureValue + currentFeatureValue) / 2.0;
                    var dataSubset = dataToSplit.GetSubsetByRows(gatheredRows);
                    var subsetPercentage = dataSubset.RowCount / totalRowsCount;
                    rangeEnd = double.PositiveInfinity;
                    var splitLink = new MultiValueNumericRangeLink(subsetPercentage, dataSubset.RowCount, halfWayValue, rangeStart, rangeEnd);
                    numericRangeSplits.Add(new SplittedData(splitLink, dataSubset));
                    rangeStart = halfWayValue;
                    previousClass = currentClass;
                    gatheredRows.Clear();
                    gatheredRows.Add(value.RowIndex);
                }
                else
                {
                    gatheredRows.Add(value.RowIndex);
                    rangeEnd = currentFeatureValue;
                }

                previousFeatureValue = currentFeatureValue;
            }

            if (gatheredRows.Any())
            {
                var halfWayValue = (previousFeatureValue + currentFeatureValue) / 2.0;
                rangeEnd = double.PositiveInfinity;
                var dataSubset = dataToSplit.GetSubsetByRows(gatheredRows);
                var subsetPercentage = dataSubset.RowCount / totalRowsCount;
                var splitLink = new MultiValueNumericRangeLink(subsetPercentage, dataSubset.RowCount, halfWayValue, rangeStart, rangeEnd);
                numericRangeSplits.Add(new SplittedData(splitLink, dataSubset));
            }

            return numericRangeSplits;
        }
    }
}
