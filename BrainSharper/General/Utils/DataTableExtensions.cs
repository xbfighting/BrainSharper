using System.Collections.Generic;
using System.Data;
using System.Linq;
using MathNet.Numerics.LinearAlgebra;

namespace BrainSharper.General.Utils
{
    public static class DataTableExtensions
    {
        public static bool IsEqualTo(this DataTable source, DataTable other)
        {
            if (other == null)
            {
                return false;
            }
            return source.AsEnumerable().SequenceEqual(other.AsEnumerable(), DataRowComparer.Default);
        }

        public static DataTable ToDataTable(this Matrix<double> matrix, IList<string> columnNames = null)
        {
            var dataTable = new DataTable();
            for (var colIdx = 0; colIdx < matrix.ColumnCount; colIdx++)
            {
                var columnName = (columnNames?[colIdx]) ?? string.Format("Col{0}", colIdx);
                dataTable.Columns.Add(new DataColumn(columnName, typeof (double)));
            }
            for (var rowIdx = 0; rowIdx < matrix.RowCount; rowIdx++)
            {
                dataTable.Rows.Add(matrix.Row(rowIdx).Cast<object>().ToArray());
            }
            return dataTable;
        }
    }
}