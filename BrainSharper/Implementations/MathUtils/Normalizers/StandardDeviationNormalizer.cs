using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using BrainSharper.Abstract.MathUtils.Normalizers;
using MathNet.Numerics.Integration;
using MathNet.Numerics.LinearAlgebra;
using MathNet.Numerics.Statistics;

namespace BrainSharper.Implementations.MathUtils.Normalizers
{
    public class StandardDeviationNormalizer : IQuantitativeDataNormalizer
    {
        public Matrix<double> NormalizeColumns(Matrix<double> dataToNormalize, IList<int> columnsToNormalize = null)
        {
            columnsToNormalize = columnsToNormalize ?? Enumerable.Range(0, dataToNormalize.ColumnCount).ToList();
            var normalizedColumns = new ConcurrentBag<Tuple<int, Vector<double>>>();
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
                    var columnStd = columnVector.StandardDeviation();
                    var columnMean = columnVector.Mean();
                    vectorToAdd = columnVector.Subtract(columnMean).Divide(columnStd);
                }
                normalizedColumns.Add(new Tuple<int, Vector<double>>(colIdx, vectorToAdd));
            });
            return Matrix<double>.Build.DenseOfColumnVectors(normalizedColumns.OrderBy(tpl => tpl.Item1).Select(tpl => tpl.Item2));
        }
    }
}
