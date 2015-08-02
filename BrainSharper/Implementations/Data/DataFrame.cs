namespace BrainSharper.Implementations.Data
{
    using System;
    using System.Collections.Concurrent;
    using System.Collections.Generic;
    using System.Data;
    using System.Linq;
    using System.Threading.Tasks;
    using Abstract.Data;
    using General.Exceptions.Data;
    using General.Utils;
    using MathNet.Numerics.LinearAlgebra;

    public class DataFrame : IDataFrame
    {
        #region Fields

        protected readonly DataTable DataTable;
        private readonly object locker = new object();
        private IList<int> rowIndices;
        private readonly Lazy<Matrix<double>> numericMatrix; 

        #endregion Fields

        #region Constructors

        public DataFrame(DataTable dataTable, IList<int> rowIndices = null)
        {
            DataTable = dataTable;
            if (rowIndices == null)
            {
                this.rowIndices = dataTable.AsEnumerable().Select((row, rowIdx) => (int)rowIdx).ToList();
            }
            else
            {
                this.rowIndices = rowIndices.Take(dataTable.Rows.Count).ToList();
            }
            this.numericMatrix = new Lazy<Matrix<double>>(CreateMatrixFromDataTable);
        }

        public DataFrame(Matrix<double> matrix, IList<string> columnNames = null, IList<int> rowIndices = null)
            : this(matrix.ToDataTable(columnNames), rowIndices)
        {

        }

        #endregion Constructors

        #region Getters/setters

        public DataTable InnerTable => DataTable;

        public IList<Type> ColumnTypes => (from DataColumn col in InnerTable.Columns
                                           select col.DataType).ToList();

        public IList<string> ColumnNames
        {
            get
            {
                return (from DataColumn col in DataTable.Columns
                        select col.ColumnName).ToList();
            }
            set
            {
                if (DataTable.Columns.Count == 0)
                {
                    AddNewColumns(value);
                }
                else
                {
                    AlterExistingColumns(value.ToList());
                }
            }
        }

        public IList<int> RowIndices
        {
            get { return this.rowIndices; }
            set { this.rowIndices = value.Count() != DataTable.Rows.Count ? value.Take(DataTable.Rows.Count).ToList() : value; }
        }

        public bool Any => InnerTable != null && InnerTable.Rows.Count > 0;

        public int RowCount => DataTable.Rows.Count;

        public int ColumnsCount => DataTable.Columns.Count;

        IDataItem<object> IDataFrame.this[int rowIdx, int columnIdx] => new DataItem<object>(ColumnNames[(int)columnIdx], DataTable.Rows[(int)rowIdx][columnIdx]);

        public IDataItem<object> this[int rowIdx, string columnName]
        {
            get
            {
                if (!ColumnNames.Contains(columnName))
                {
                    throw new ArgumentException(string.Format("Invalid column name: {0}", columnName));
                }
                return new DataItem<object>(columnName, DataTable.Rows[(int)rowIdx][columnName]);
            }
        }

        public string Name
        {
            get { return DataTable.TableName; }
            set { DataTable.TableName = value; }
        }

        #endregion Getters/setters

        #region Slicers

        public Type GetColumnType(string columnName)
        {
            return InnerTable.Columns[columnName].DataType;
        }

        public Type GetColumnType(int columnIdx)
        {
            return InnerTable.Columns[columnIdx].DataType;
        }

        public IDataVector<TValue> GetRowVector<TValue>(int index, bool useRowNames = false)
        {
            index = useRowNames ? RowNameToIndex(index) : index;
            return new DataVector<TValue>(
                DataTable.Rows[index].ItemArray.Select(itm => (TValue)Convert.ChangeType(itm, typeof(TValue))).ToList(),
                ColumnNames);
        }

        public IDataVector<object> GetRowVector(int index, bool useRowNames = false)
        {
            index = useRowNames ? RowNameToIndex(index) : index;
            return new DataVector<object>(
                DataTable.Rows[index].ItemArray,
                ColumnNames);
        }

        public Vector<double> GetNumericRowVector(int index, bool useRowNames = false)
        {
            return GetRowVector(index, useRowNames).NumericVector;
        }

        public IDataVector<TValue> GetColumnVector<TValue>(int columnIndex)
        {
            var elements = (
                                from row in DataTable.AsEnumerable().AsParallel().AsOrdered()
                                select (TValue)Convert.ChangeType(row[columnIndex], typeof(TValue))
                                ).ToList();
            return new DataVector<TValue>(elements, ColumnNames[columnIndex]);
        }

        public IDataVector<TValue> GetColumnVector<TValue>(string columnName)
        {
            return GetColumnVector<TValue>(ColumnNames.IndexOf(columnName));
        }

        public IDataVector<object> GetColumnVector(int columnIndex)
        {
            var values = (
                from row in DataTable.AsEnumerable().AsParallel().AsOrdered()
                select row[columnIndex]
                ).ToList();
            return new DataVector<object>(values, ColumnNames[columnIndex]);
        }

        public IDataVector<object> GetColumnVector(string columnName)
        {
            return GetColumnVector(ColumnNames.IndexOf(columnName));
        }

        public Vector<double> GetNumericColumnVector(int columnIndex)
        {
            return GetColumnVector<double>(columnIndex).NumericVector;
        }

        public Vector<double> GetNumericColumnVector(string columnName)
        {
            return GetColumnVector<double>(columnName).NumericVector;
        }

        public IDataFrame GetSubsetByColumns(IList<string> columnNames)
        {
            return new DataFrame(DataTable.DefaultView.ToTable(false, columnNames.ToArray()), RowIndices);
        }

        public IDataFrame GetSubsetByColumns(IList<int> columnIndices)
        {
            return GetSubsetByColumns(columnIndices.Select(idx => ColumnNames[idx]).ToList());
        }

        public IDataFrame GetSubsetByRows(IList<int> rowIndices, bool useRowNames = false)
        {
            var newTable = new DataTable();
            foreach (DataColumn col in DataTable.Columns)
            {
                newTable.Columns.Add(new DataColumn(col.ColumnName, col.DataType));
            }
            rowIndices = useRowNames ? RowNamesToRowIndices(rowIndices) : rowIndices;
            foreach (var rowIdx in rowIndices)
            {
                newTable.Rows.Add(DataTable.Rows[rowIdx].ItemArray);
            }
            return new DataFrame(newTable, RowIndices.Where((rowName, rowIdx) => rowIndices.Contains(rowIdx)).ToList());
        }

        public IList<TValue> GetValuesForRows<TValue>(IList<int> rowIndices, string columnName, bool useRowName = false)
        {
            var results = new ConcurrentBag<Tuple<int, TValue>>();
            var indices = useRowName ? RowNamesToRowIndices(rowIndices) : rowIndices;
            Parallel.ForEach(indices, rowIdx =>
            {
                    var row = InnerTable.Rows[rowIdx];
                    var rowValue = (TValue)row[columnName];
                    results.Add(new Tuple<int, TValue>(rowIdx, rowValue));
            });
            return results.OrderBy(tpl => tpl.Item1).Select(tpl => tpl.Item2).ToList();
        }

        public IList<TValue> GetValuesForRows<TValue>(IList<int> rowIndices, int columnIndex, bool useRowName = false)
        {
            return GetValuesForRows<TValue>(rowIndices, ColumnNames[columnIndex], useRowName);
        }

        public IDataFrame Slice(IList<int> rows, IList<string> columns, bool useRowNames = false)
        {
            return GetSubsetByRows(rows, useRowNames).GetSubsetByColumns(columns);
        }

        public IDataFrame Slice(IList<int> rows, IList<int> columnIndices, bool useRowNames = false)
        {
            return GetSubsetByRows(rows, useRowNames).GetSubsetByColumns(columnIndices);
        }

        public IDataFrame Set<TValue>(TValue value, int rowIndex, int columnIndex, bool useRowNames = false)
        {
            var newDataTable = DataTable.DefaultView.ToTable();
            var idx = useRowNames ? RowNameToIndex(rowIndex) : rowIndex;
            newDataTable.Rows[idx][columnIndex] = value;
            return new DataFrame(newDataTable, new List<int>(RowIndices));
        }

        public IDataFrame Set<TValue>(TValue value, int rowIndex, string columnName, bool useRowNames = false)
        {
            return Set(value, rowIndex, ColumnNames.IndexOf(columnName), useRowNames);
        }

        public IDataFrame ProcessMultiple<TValue>(DataFrameRowIndexColumnNameOperator<TValue> rowOperator)
        {
            var newData = DataTable.DefaultView.ToTable();
            Parallel.For(0, newData.Rows.Count, rowIdx =>
            {
                var row = newData.Rows[rowIdx];
                for (int colIdx = 0; colIdx < row.ItemArray.Length; colIdx++)
                {
                    var newVal = rowOperator(rowIdx, ColumnNames[colIdx], (TValue)Convert.ChangeType(row[colIdx], typeof(TValue)));
                    lock (this.locker)
                    {
                        row[colIdx] = newVal;
                    }
                }
            });
            return new DataFrame(newData, new List<int>(RowIndices));
        }

        public IDataFrame ProcessMultiple<TValue>(DataFrameRowIndexColumnIndexOperator<TValue> rowOperator)
        {
            var newData = DataTable.DefaultView.ToTable();
            Parallel.For(0, newData.Rows.Count, rowIdx =>
            {
                var row = newData.Rows[rowIdx];
                for (int colIdx = 0; colIdx < ColumnNames.Count; colIdx++)
                {
                    var newVal = rowOperator(rowIdx, colIdx, (TValue)Convert.ChangeType(row[colIdx], typeof(TValue)));
                    lock (this.locker)
                    {
                        newData.Rows[rowIdx][colIdx] = newVal;
                    }
                }
            });
            return new DataFrame(newData, new List<int>(RowIndices));
        }

        public IDataFrame ProcessMultiple<TValue>(DataFrameRowNameColumnIndexOperator<TValue> rowOperator)
        {
            var newData = DataTable.DefaultView.ToTable();
            Parallel.For(0, newData.Rows.Count, rowIdx =>
            {
                var row = newData.Rows[rowIdx];
                var rowName = this.rowIndices[rowIdx];
                for (int colIdx = 0; colIdx < row.ItemArray.Length; colIdx++)
                {
                    var newVal = rowOperator(rowName, colIdx, (TValue)Convert.ChangeType(row[colIdx], typeof(TValue)));
                    lock (this.locker)
                    {
                        row[colIdx] = newVal;
                    }
                }
            });
            return new DataFrame(newData, new List<int>(RowIndices));
        }

        public IDataFrame ProcessMultiple<TValue>(DataFrameRowNameColumnNameOperator<TValue> rowOperator)
        {
            var newData = DataTable.DefaultView.ToTable();
            Parallel.For(0, newData.Rows.Count, rowIdx =>
            {
                var row = newData.Rows[rowIdx];
                var rowName = this.rowIndices[rowIdx];
                for (int colIdx = 0; colIdx < row.ItemArray.Length; colIdx++)
                {
                    var newVal = rowOperator(rowName, ColumnNames[colIdx], (TValue)Convert.ChangeType(row[colIdx], typeof(TValue)));
                    lock (this.locker)
                    {
                        row[colIdx] = newVal;
                    }
                }
            });
            return new DataFrame(newData, new List<int>(RowIndices));
        }

        public IFilteringResult GetRowsIndicesWhere(Predicate<DataRow> rowsFilter)
        {
            var rowsMeetingCriteria = new ConcurrentBag<int>();
            var rowsNotMeetingCriteria = new ConcurrentBag<int>();
            Parallel.For(0, RowCount, rowIdx =>
            {
                var row = DataTable.Rows[rowIdx];
                if (rowsFilter(row))
                {
                    rowsMeetingCriteria.Add(rowIdx);
                }
                else
                {
                    rowsNotMeetingCriteria.Add(rowIdx);
                }
            });
            return new FilteringResult(
                rowsMeetingCriteria.OrderBy(i => i).ToList(),
                rowsNotMeetingCriteria.OrderBy(i => i).ToList());
        }

        public Matrix<double> GetAsMatrix()
        {
            return this.numericMatrix.Value;
        }

        #endregion Slicers

        #region Private methods

        private void AlterExistingColumns(IList<string> value)
        {
            if (value.Count() != DataTable.Columns.Count)
            {
                throw new InvalidColumnNamesException();
            }
            var valuesList = value.ToList();
            for (int i = 0; i < valuesList.Count; i++)
            {
                DataTable.Columns[i].ColumnName = valuesList[i];
            }
        }

        private void AddNewColumns(IEnumerable<string> value)
        {
            foreach (var colName in value)
            {
                DataTable.Columns.Add(new DataColumn(colName));
            }
        }

        private IList<int> RowNamesToRowIndices(IList<int> rowNames)
        {
            return rowNames.Select(RowNameToIndex).ToList();
        }

        private int RowNameToIndex(int rowName)
        {
            return RowIndices.IndexOf(rowName);
        }

        private Matrix<double> CreateMatrixFromDataTable()
        {
            var rowsArray = DataTable.AsEnumerable().Select(row => row.ItemArray.Select(Convert.ToDouble).ToArray()).ToList();
            return Matrix<double>.Build.DenseOfRowArrays(rowsArray);
        }


        #endregion Private methods

        #region Equality members

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != this.GetType()) return false;
            return Equals((DataFrame)obj);
        }

        public override int GetHashCode()
        {
            unchecked
            {
                var hash = 0;
                if (DataTable != null)
                {
                    foreach (DataRow row in DataTable.Rows)
                    {
                        hash = row.ItemArray.Aggregate(hash, (acc, elem) => acc ^ elem.GetHashCode() * 397);
                    }
                }

                if (this.rowIndices != null)
                {
                    hash = this.rowIndices.Aggregate(hash, (acc, elem) => acc ^ elem.GetHashCode() * 397);
                }
                return hash;
            }
        }

        protected bool Equals(DataFrame other)
        {
            var otherDataTable = other.DataTable ?? new DataTable();
            var otherRowIndices = other.RowIndices ?? new List<int>();
            return DataTable.IsEqualTo(otherDataTable) && otherRowIndices.SequenceEqual(RowIndices);
        }

        #endregion Equality members
    }
}
