namespace BrainSharper.Implementations.Algorithms.DecisionTrees.Processors
{
    using System;
    using System.Collections.Concurrent;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;

    using Abstract.Algorithms.DecisionTrees.DataStructures;
    using Abstract.Algorithms.DecisionTrees.Processors;
    using Abstract.Data;
    using DataStructures;

    public class MultiValueDiscreteDataSplitter<TSplitValue> : IDataSplitter<TSplitValue>
    {
        public IList<ISplittedData<TSplitValue>> SplitData(IDataFrame dataToSplit, ISplittingParams splttingParams)
        {
            var splittingFeature = splttingParams.SplitOnFeature;
            var distinctValues = dataToSplit.GetColumnVector<TSplitValue>(splittingFeature).Distinct();
            var results = new ConcurrentBag<Tuple<TSplitValue, int>>();
            Parallel.For(0, dataToSplit.RowCount, rowIdx =>
            {
                var row = dataToSplit.GetRowVector<TSplitValue>(rowIdx);
                var rowValue = row[splittingFeature];
                results.Add(new Tuple<TSplitValue, int>(rowValue, rowIdx));
            });
            var totalRowsCount = dataToSplit.RowCount;

            var resultsGroupedByValue = from result in results
                                        let rowIdx = result.Item2
                                        let groupKey = result.Item1
                                        group rowIdx by groupKey
                                        into grp
                                        select grp;
            return (
                from grp in resultsGroupedByValue
                let grpCount = grp.Count()
                select new SplittedData<TSplitValue>(
                    new DecisionLink<TSplitValue>(CalcInstancesPercentage(totalRowsCount, grpCount), grpCount, grp.Key),
                    dataToSplit.GetSubsetByRows(grp.ToList())
                    ) as ISplittedData<TSplitValue>).ToList();
        }

        private double CalcInstancesPercentage(int totalRowsCount, int splitRowsCount)
        {
            return splitRowsCount/(double) totalRowsCount;
        }
    }
}
