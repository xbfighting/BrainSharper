namespace BrainSharper.Implementations.Algorithms.AssociationAnalysis.DataStructures
{
    using System;
    using System.Collections.Concurrent;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;

    using BrainSharper.Abstract.Algorithms.AssociationAnalysis.DataStructures;
    using BrainSharper.Abstract.Data;

    public static class DataUtils
    {
        public static ITransactionsSet<IDataItem<TValue>> ToAssociativeTransactionsSet<TValue>(
            this IDataFrame dataFrame,
            string keyColumn,
            IEnumerable<string> columnsToExchange = null)
        {
            columnsToExchange = columnsToExchange ?? dataFrame.ColumnNames;
            return dataFrame.ToAssociativeTransactionsSet<TValue>(
                dataFrame.ColumnNames.IndexOf(keyColumn),
                columnsToExchange.Select(name => dataFrame.ColumnNames.IndexOf(name)));
        }

        public static ITransactionsSet<IDataItem<TValue>> ToAssociativeTransactionsSet<TValue>(
            this IDataFrame dataFrame,
            int keyColumn,
            IEnumerable<int> columnIndicesToChange = null)
        {
            var keyColName = dataFrame.ColumnNames[keyColumn];
            columnIndicesToChange = columnIndicesToChange ?? dataFrame.ColumnNames.Select((_, idx) => idx).ToList();
            var transactionsDictionary = new ConcurrentDictionary<object, List<IDataItem<TValue>>>();
            Parallel.ForEach(
                dataFrame.RowIndices,
                rowIdx =>
                    {
                        var transactionId = dataFrame[rowIdx, keyColumn].FeatureValue;
                        var itemsToTake = dataFrame.GetRowVector<TValue>(rowIdx).DataItems.Where(
                            (itm, idx) => columnIndicesToChange.Contains(idx) && itm.FeatureName != keyColName).ToList();
                        transactionsDictionary.AddOrUpdate(
                            transactionId,
                            addValue: itemsToTake,
                            updateValueFactory: (o, existingList) => existingList.Union(itemsToTake).ToList());
                    });
            return new TransactionsSet<IDataItem<TValue>>(
                transactionsDictionary.Select(kvp => new Transaction<IDataItem<TValue>>(kvp.Key, kvp.Value) as ITransaction<IDataItem<TValue>>)
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
