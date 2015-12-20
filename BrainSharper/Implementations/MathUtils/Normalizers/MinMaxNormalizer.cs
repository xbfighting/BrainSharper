using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using BrainSharper.Abstract.MathUtils.Normalizers;
using MathNet.Numerics.LinearAlgebra;

namespace BrainSharper.Implementations.MathUtils.Normalizers
{
    public class MinMaxNormalizer : IQuantitativeDataNormalizer
    {
        public Matrix<double> NormalizeColumns(Matrix<double> dataToNormalize, IList<int> columnsToNormalize = null)
        {
            columnsToNormalize = columnsToNormalize ?? Enumerable.Range(0, dataToNormalize.ColumnCount).ToList();
            var normalizedColumns = new ConcurrentBag<Tuple<int, Vector<double>>>();
            var columnMins = new double[dataToNormalize.ColumnCount];
            var columnRanges = new double[dataToNormalize.ColumnCount];
            for (var colIdx = 0; colIdx < dataToNormalize.ColumnCount; colIdx++)
            {
                var columnMin = dataToNormalize.Column(colIdx).Minimum();
                var columnMax = dataToNormalize.Column(colIdx).Maximum();
                var colRange = columnMax - columnMin;

                columnMins[colIdx] = columnMin;
                columnRanges[colIdx] = colRange;
            }
            Parallel.For(0, dataToNormalize.ColumnCount, colIdx =>
            {
                Vector<double> vectorToAdd;
                if (!columnsToNormalize.Contains(colIdx))
                {
                    vectorToAdd = dataToNormalize.Column(colIdx);
                }
                else
                {
                    var columnVector = dataToNormalize.Column(colIdx);
                    var columnMin = columnMins[colIdx];
                    var colRange = columnRanges[colIdx];
                    vectorToAdd = columnVector.Subtract(columnMin).Divide(colRange);
                }
                normalizedColumns.Add(new Tuple<int, Vector<double>>(colIdx, vectorToAdd));
            });

            return
                Matrix<double>.Build.DenseOfColumnVectors(
                    normalizedColumns.OrderBy(tpl => tpl.Item1).Select(tpl => tpl.Item2));
        }
    }
}