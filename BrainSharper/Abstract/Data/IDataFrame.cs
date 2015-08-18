namespace BrainSharper.Abstract.Data
{
    #region

    using System;
    using System.Collections.Generic;
    using System.Data;

    using MathNet.Numerics.LinearAlgebra;

    #endregion

    public delegate TValue DataFrameRowIndexColumnNameOperator<TValue>(
        int rowIndex, 
        string columnName, 
        TValue actualValue);

    public delegate TValue DataFrameRowIndexColumnIndexOperator<TValue>(
        int rowIndex, 
        int columnIndex, 
        TValue actualValue);

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

        /// <summary>
        /// Checks if dataframe contains any data
        /// </summary>
        bool Any { get; }

        int RowCount { get; }

        int ColumnsCount { get; }

        IDataItem<object> this[int rowIdx, int columnIdx] { get; }

        IDataItem<object> this[int rowIdx, string columnName] { get; }

        Type GetColumnType(string columnName);

        Type GetColumnType(int columnIdx);

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

        /// <summary>
        /// Gets subset of IDataFrame - another IDataFrame - based on the selected rows.
        /// </summary>
        /// <param name="rowIndices">Int indices for rows to take</param>
        /// <param name="useRowNames">Treat indices as rownames</param>
        /// <returns>IDataFrame contatining only selected rows</returns>
        IDataFrame GetSubsetByRows(IList<int> rowIndices, bool useRowNames = false);

        /// <summary>
        /// Performs sql-like query to the underlying storage. A simple, standard SQL query syntax should be used
        /// </summary>
        /// <param name="query">Simple sql query.</param>
        /// <returns>IDataFrame</returns>
        IDataFrame GetSubsetByQuery(string query);

        /// <summary>
        /// Gets values for selected rows from a selected column.
        /// </summary>
        /// <typeparam name="TValue">Generic type to be returned</typeparam>
        /// <param name="rowIndices">Indices of rows to be processed</param>
        /// <param name="columnName">Name of the column from which values should be extracted</param>
        /// <param name="useRowName">Should row names be used instead of indices</param>
        /// <returns>List of values</returns>
        IList<TValue> GetValuesForRows<TValue>(IList<int> rowIndices, string columnName, bool useRowName = false);

        /// <summary>
        /// Gets values for selected rows from a selected column.
        /// </summary>
        /// <typeparam name="TValue">Generic type to be returned</typeparam>
        /// <param name="rowIndices">Indices of rows to be processed</param>
        /// <param name="columnIndex">Index of the column from which values should be extracted</param>
        /// <param name="useRowName">Should row names be used instead of indices</param>
        /// <returns>List of values</returns>
        IList<TValue> GetValuesForRows<TValue>(IList<int> rowIndices, int columnIndex, bool useRowName = false);

        IDataFrame Slice(IList<int> rows, IList<string> columns, bool useRowNames = false);

        IDataFrame Slice(IList<int> rows, IList<int> columnIndices, bool useRowNames = false);

        IDataFrame Set<TValue>(TValue value, int rowIndex, int columnIndex, bool useRowNames = false);

        IDataFrame Set<TValue>(TValue value, int rowIndex, string columnName, bool useRowNames = false);

        IDataFrame ProcessMultiple<TValue>(DataFrameRowIndexColumnNameOperator<TValue> rowOperator);

        IDataFrame ProcessMultiple<TValue>(DataFrameRowIndexColumnIndexOperator<TValue> rowOperator);

        IDataFrame ProcessMultiple<TValue>(DataFrameRowNameColumnIndexOperator<TValue> rowOperator);

        IDataFrame ProcessMultiple<TValue>(DataFrameRowNameColumnNameOperator<TValue> rowOperator);

        IFilteringResult GetRowsIndicesWhere(Predicate<DataRow> rowsFilter);

        Matrix<double> GetAsMatrix();

        Matrix<double> GetAsMatrixWithIntercept();

        /// <summary>
        /// Compares two DataFrames using only internal table storage, ignoring the row names.
        /// </summary>
        /// <param name="other">Other data frame</param>
        /// <returns>True or false</returns>
        bool ContentEquals(IDataFrame other);
    }
}