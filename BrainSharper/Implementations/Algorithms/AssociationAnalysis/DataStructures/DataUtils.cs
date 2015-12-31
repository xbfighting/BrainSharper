using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using BrainSharper.Abstract.Algorithms.AssociationAnalysis.DataStructures;
using BrainSharper.Abstract.Data;

namespace BrainSharper.Implementations.Algorithms.AssociationAnalysis.DataStructures
{
    public static class DataUtils
    {
        public static ITransactionsSet<IDataItem<TValue>> ToAssociativeTransactionsSet<TValue>(
            this IDataFrame dataFrame,
            string keyColumn = null,
            IEnumerable<string> columnsToExchange = null)
        {
            columnsToExchange = columnsToExchange ?? dataFrame.ColumnNames;
            return dataFrame.ToAssociativeTransactionsSet<TValue>(
                keyColumn == null ? (int?) null : dataFrame.ColumnNames.IndexOf(keyColumn),
                columnsToExchange.Select(name => dataFrame.ColumnNames.IndexOf(name)));
        }

        public static ITransactionsSet<IDataItem<TValue>> ToAssociativeTransactionsSet<TValue>(
            this IDataFrame dataFrame,
            int? keyColumn,
            IEnumerable<int> columnIndicesToChange = null)
        {
            var keyColName = keyColumn.HasValue ? dataFrame.ColumnNames[keyColumn.Value] : "Id";
            columnIndicesToChange = columnIndicesToChange ?? dataFrame.ColumnNames.Select((_, idx) => idx).ToList();
            var transactionsDictionary = new ConcurrentDictionary<object, List<IDataItem<TValue>>>();
            Parallel.ForEach(
                dataFrame.RowIndices,
                rowIdx =>
                {
                    var transactionId = keyColumn.HasValue ? dataFrame[rowIdx, keyColumn.Value].FeatureValue : rowIdx;
                    var itemsToTake = dataFrame.GetRowVector<TValue>(rowIdx).DataItems.Where(
                        (itm, idx) => columnIndicesToChange.Contains(idx) && itm.FeatureName != keyColName).ToList();
                    transactionsDictionary.AddOrUpdate(
                        transactionId, itemsToTake, (o, existingList) => existingList.Union(itemsToTake).ToList());
                });
            return new TransactionsSet<IDataItem<TValue>>(
                transactionsDictionary.Select(
                    kvp => new Transaction<IDataItem<TValue>>(kvp.Key, kvp.Value) as ITransaction<IDataItem<TValue>>)
                );
        }

        public static ITransactionsSet<IDataItem<object>> ToAssociativeTransactionsSet(
            this IDataFrame dataFrame,
            string keyColumn,
            IEnumerable<string> columnsToExchange = null)
        {
            return dataFrame.ToAssociativeTransactionsSet<object>(
                keyColumn,
                columnsToExchange);
        }

        public static ITransactionsSet<IDataItem<object>> ToAssociativeTransactionsSet(
            this IDataFrame dataFrame,
            int keyColumn,
            IEnumerable<int> columnIndicesToChange = null)
        {
            return dataFrame.ToAssociativeTransactionsSet<object>(keyColumn, columnIndicesToChange);
        }
    }
}