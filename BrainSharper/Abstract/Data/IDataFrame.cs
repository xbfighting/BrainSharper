using System;
using System.Collections.Generic;
using MathNet.Numerics.LinearAlgebra;

namespace BrainSharper.Abstract.Data
{
    public delegate TValue DataFrameRowIndexColumnNameOperator<TValue>(int rowIndex, string columnName, TValue actualValue);
    public delegate TValue DataFrameRowIndexColumnIndexOperator<TValue>(int rowIndex, int columnIndex, TValue actualValue);

    public delegate TValue DataFrameRowNameColumnNameOperator<TValue>(int rowName, string columnName, TValue actualValue);
    public delegate TValue DataFrameRowNameColumnIndexOperator<TValue>(int rowName, int columnIndex, TValue actualValue);

    /// <summary>
    /// Basic interface representing excel/sql similar data structure - columns represent features,
    /// rows represent instances.
    /// </summary>
    public interface IDataFrame
    {
        /// <summary>
        /// Gets or sets data frame name
        /// </summary>
        string Name { get; set; }

        /// <summary>
        /// Data types stored in the data frame
        /// </summary>
        IList<Type> ColumnTypes { get; }

        /// <summary>
        /// Gets or sets column names. Throws exception if new column names count is not the same
        /// as existing columns names.
        /// </summary>
        IList<string> ColumnNames { get; set; }

        /// <summary>
        /// Indices of the rows. They represent natural order of rows - so numbers are not custom and user-defined
        /// </summary>
        IList<int> RowIndices { get; set; }

        int RowCount { get; }
        int ColumnsCount { get; }

        IDataItem<object> this[int rowIdx, int columnIdx] {get;}
        IDataItem<object> this[int rowIdx, string columnName] { get; }

        /// <summary>
        /// Gets selected row as IDataVector of TValue type
        /// </summary>
        /// <typeparam name="TValue">Generic type of vector to be returned</typeparam>
        /// <param name="index">Index of row/Rowname</param>
        /// <param name="useRowNames">Should the number passed in 'index' parameter interpreted as row name or natural row index?</param>
        /// <returns>IDataVector of generic type TValue</returns>
        IDataVector<TValue> GetRowVector<TValue>(int index, bool useRowNames = false);

        /// <summary>
        /// Gets selected row as IDataVector of objects
        /// </summary>
        /// <param name="index">Index of row/Rowname</param>
        /// <param name="useRowNames">Should the number passed in 'index' parameter interpreted as row name or natural row index?</param>
        /// <returns>IDataVector of generic type TValue</returns>
        IDataVector<object> GetRowVector(int index, bool useRowNames = false);
        Vector<double> GetNumericRowVector(int index, bool useRowNames = false);

        /// <summary>
        /// Get vector of data, based on the seleced column in a dataframe
        /// </summary>
        /// <typeparam name="TValue">Generic type to be returned</typeparam>
        /// <param name="columnIndex">Column index</param>
        /// <returns>IDataVector of generic type TValue</returns>
        IDataVector<TValue> GetColumnVector<TValue>(int columnIndex);

        /// <summary>
        /// Get vector of data, based on the seleced column in a dataframe
        /// </summary>
        /// <typeparam name="TValue">Generic type to be returned</typeparam>
        /// <param name="columnName">Column name</param>
        /// <returns>IDataVector of generic type TValue</returns>
        IDataVector<TValue> GetColumnVector<TValue>(string columnName);

        /// <summary>
        /// Get vector of data, based on the seleced column in a dataframe
        /// </summary>
        /// <param name="columnIndex">Column index</param>
        /// <returns>IDataVector of objects</returns>
        IDataVector<object> GetColumnVector(int columnIndex);

        /// <summary>
        /// Get vector of data, based on the seleced column in a dataframe
        /// </summary>
        /// <param name="columnName">Column name</param>
        /// <returns>IDataVector of objects</returns>
        IDataVector<object> GetColumnVector(string columnName);

        /// <summary>
        /// Tries to get the column as Math.NET Numerics Vector of doubles
        /// </summary>
        /// <returns>Vector of doubles</returns>
        Vector<double> GetNumericColumnVector(int columnIndex);

        /// <summary>
        /// Tries to get the column as Math.NET Numerics Vector of doubles
        /// </summary>
        /// <returns>Vector of doubles</returns>
        Vector<double> GetNumericColumnVector(string columnName);

        /// <summary>
        /// Get subset of IDataFrame - another IDataFrame - based on the selected columns
        /// </summary>
        /// <param name="columnNames">Column names to be used</param>
        /// <returns>IDataFrame containing only selected columns</returns>
        IDataFrame GetSubsetByColumns(IList<string> columnNames);

        /// <summary>
        /// Get subset of IDataFrame - another IDataFrame - based on the selected columns
        /// </summary>
        /// <param name="columnIndices">Column indices to be used</param>
        /// <returns>IDataFrame containing only selected columns</returns>
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
