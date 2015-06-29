using System.Collections.Generic;
using System.Data;
using MathNet.Numerics.LinearAlgebra;

namespace BrainSharper.Abstract.Data
{
    public delegate TValue DataFrameRowIndexColumnNameOperator<TValue>(int rowIndex, string columnName, TValue actualValue);
    public delegate TValue DataFrameRowIndexColumnIndexOperator<TValue>(int rowIndex, int columnIndex, TValue actualValue);

    public delegate TValue DataFrameRowNameColumnNameOperator<TValue>(int rowName, string columnName, TValue actualValue);
    public delegate TValue DataFrameRowNameColumnIndexOperator<TValue>(int rowName, int columnIndex, TValue actualValue);

    public interface IDataFrame
    {
        /// <summary>
        /// Gets or sets data frame name
        /// </summary>
        string Name { get; set; }

        DataTable InnerTable { get; }

        /// <summary>
        /// Gets or sets column names. Throws exception if new column names count is not the same
        /// as existing columns names.
        /// </summary>
        IList<string> ColumnNames { get; set; }
        IList<int> RowIndices { get; set; }
        int RowCount { get; }
        int ColumnsCount { get; }

        IDataItem<object> this[int rowIdx, int columnIdx] {get;}
        IDataItem<object> this[int rowIdx, string columnName] { get; }

        IDataVector<TValue> GetRowVector<TValue>(int index, bool useRowNames = false);
        IDataVector<object> GetRowVector(int index, bool useRowNames = false);
        Vector<double> GetNumericRowVector(int index, bool useRowNames = false);

        IDataVector<TValue> GetColumnVector<TValue>(int columnIndex);
        IDataVector<TValue> GetColumnVector<TValue>(string columnName);
        IDataVector<object> GetColumnVector(int columnIndex);
        IDataVector<object> GetColumnVector(string columnName);
        Vector<double> GetNumericColumnVector(int columnIndex);
        Vector<double> GetNumericColumnVector(string columnName);

        IDataFrame GetSubsetByColumns(IList<string> columnNames);
        IDataFrame GetSubsetByColumns(IList<int> columnIndices);
        IDataFrame GetSubsetByRows(IList<int> rowIndices, bool useRowNames = false);
        IDataFrame Slice(IList<int> rows, IList<string> columns, bool useRowNames = false);
        IDataFrame Slice(IList<int> rows, IList<int> columnIndices, bool useRowNames = false);

        IDataFrame Set<TValue>(TValue value, int rowIndex, int columnIndex, bool useRowNames = false);
        IDataFrame Set<TValue>(TValue value, int rowIndex, string columnName, bool useRowNames = false);
        IDataFrame ProcessMultiple<TValue>(DataFrameRowIndexColumnNameOperator<TValue> rowOperator);
        IDataFrame ProcessMultiple<TValue>(DataFrameRowIndexColumnIndexOperator<TValue> rowOperator);
        IDataFrame ProcessMultiple<TValue>(DataFrameRowNameColumnIndexOperator<TValue> rowOperator);
        IDataFrame ProcessMultiple<TValue>(DataFrameRowNameColumnNameOperator<TValue> rowOperator);

        Matrix<double> GetAsMatrix();
    }
}
